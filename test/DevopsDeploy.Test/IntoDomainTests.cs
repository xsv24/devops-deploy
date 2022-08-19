using AutoFixture.Xunit2;
using DevopsDeploy.Domain;
using DevopsDeploy.Models;
using FluentAssertions;

namespace DevopsDeploy.Test;
public class IntoDomainTests {

    [Fact]
    public void IntoDeploymentCollection_With_The_Same_Release_Env_Project_Are_Grouped_And_Sorted() {
        // Arrange
        var now = DateTime.UtcNow;

        var projects = Fake.FakeProjects("app");
        var envs = Fake.FakeEnvs("test");
        var releases = Fake.FakeReleases(projects["app"], "new-feature");

        var deployments = new List<Deployment>();
        deployments.AddLinkedDeployment(now, envs["test"], releases["new-feature"]);
        deployments.AddLinkedDeployment(now.AddHours(1), envs["test"], releases["new-feature"]);

        var data = new DeploymentData(
            Deployments: deployments.ToHashSet(),
            Environments: envs,
            Releases: releases,
            Projects: projects
        );

        // Act
        var results = data.IntoDeploymentCollection(maxDeployments: 10);

        // Assert
        results.Should().HaveCount(1);

        var result = results.First();

        result.Value.Should().HaveCount(2);
        result.Value.Values
            .Should()
            .BeEquivalentTo(deployments.Select(dep => dep.IntoDomain(envs, releases, projects)))
            .And
            .BeInAscendingOrder(deployment => deployment.DeployedAt);
    }

    [Fact]
    public void IntoDeploymentCollection_When_Max_Deployments_Hit_The_Newest_Deployments_Are_Left() {
        // Arrange
        var now = DateTime.UtcNow;

        var projects = Fake.FakeProjects("app");
        var envs = Fake.FakeEnvs("test");
        var releases = Fake.FakeReleases(projects["app"], "new-feature");

        var deployments = new List<Deployment>();
        deployments.AddLinkedDeployment(now.AddSeconds(2), envs["test"], releases["new-feature"]);
        deployments.AddLinkedDeployment(now, envs["test"], releases["new-feature"]);
        deployments.AddLinkedDeployment(now.AddSeconds(1), envs["test"], releases["new-feature"]);

        var data = new DeploymentData(
            Deployments: deployments.ToHashSet(),
            Environments: envs,
            Releases: releases,
            Projects: projects
        );

        // Act
        var results = data.IntoDeploymentCollection(maxDeployments: 2);

        // Assert
        results.Should().HaveCount(1);

        var result = results.First();

        result.Value.Should().HaveCount(2);
        result.Value.Values.Should()
            .BeEquivalentTo(new[]
            {
                deployments[2].IntoDomain(envs, releases, projects),
                deployments[0].IntoDomain(envs, releases, projects)
            })
            .And
            .BeInAscendingOrder(deployment => deployment.DeployedAt);
    }

    [Fact]
    public void IntoDeploymentCollection_With_Different_Releases_Are_Separate_Groups() {
        // Arrange
        var now = DateTime.UtcNow;

        var envs = Fake.FakeEnvs("prod");
        var projects = Fake.FakeProjects("api");
        var releases = Fake.FakeReleases(projects["api"], "new-feature", "bug-fix");

        var deployments = new List<Deployment>();
        deployments.AddLinkedDeployment(now, envs["prod"], releases["new-feature"]);
        deployments.AddLinkedDeployment(now.AddHours(1), envs["prod"], releases["bug-fix"]);

        var data = new DeploymentData(
            Deployments: deployments.ToHashSet(),
            Environments: envs,
            Projects: projects,
            Releases: releases
        );

        // Act & Assert
        data.IntoDeploymentCollection(maxDeployments: 10)
            .Should()
            .HaveCount(2);
    }

    [Fact]
    public void IntoDeploymentCollection_With_Different_Environments_Are_Separate_Groups() {
        // Arrange
        var now = DateTime.UtcNow;

        var projects = Fake.FakeProjects("app");
        var envs = Fake.FakeEnvs("test", "prod");
        var releases = Fake.FakeReleases(projects["app"], "new-feature");

        var deployments = new List<Deployment>();
        deployments.AddLinkedDeployment(now, envs["test"], releases["new-feature"]);
        deployments.AddLinkedDeployment(now.AddHours(1), envs["prod"], releases["new-feature"]);

        var data = new DeploymentData(
            Deployments: deployments.ToHashSet(),
            Environments: envs,
            Releases: releases,
            Projects: projects
        );

        // Act & Assert
        data.IntoDeploymentCollection(maxDeployments: 10)
            .Should()
            .HaveCount(2);
    }

    [Theory, AutoData]
    public void IntoDeploymentCollection_Does_Not_Error_On_Id_Miss_Matches(DeploymentData data) {
        data.IntoDeploymentCollection(maxDeployments: Fake.FakeInt())
            .Should().BeEmpty("As none of the id's match up");
    }

    [Theory, AutoData]
    public void Deployment_IntoDomainOrDefault_With_Required_Matches(
        Deployment deployment,
        Env environment,
        Release release,
        Project project,
        Dictionary<string, Env> environments,
        Dictionary<string, Release> releases,
        Dictionary<string, Project> projects
    ) {
        // Arrange
        environments.Add(deployment.EnvironmentId.Sanitise(), environment);
        releases.Add(deployment.ReleaseId.Sanitise(), release with { ProjectId = project.Id });
        projects.Add(project.Id.Sanitise(), project);

        // Act
        var deploy = deployment.IntoDomainOrDefault(
            environments,
            releases,
            projects
        );

        // Assert
        deploy.Should().Be(new DeploymentDomain(
            Id: deployment.Id,
            DeployedAt: deployment.DeployedAt,
            Release: new ReleaseDomain(release.Id, project, release.Version, release.Created),
            Environment: environment
         ));
    }

    [Theory, AutoData]
    public void Deployment_IntoDomainOrDefault_With_No_Matching_Environment_Returns_Null(
        Deployment deployment,
        Dictionary<string, Release> releases,
        Dictionary<string, Project> projects
    ) {
        var deploy = deployment.IntoDomainOrDefault(
            new Dictionary<string, Env>(),
            releases,
            projects
        );

        deploy.Should().BeNull();
    }

    [Theory, AutoData]
    public void Deployment_IntoDomainOrDefault_With_No_Matching_Releases_Returns_Null(
        Deployment deployment,
        Env environment,
        Dictionary<string, Env> environments,
        Dictionary<string, Project> projects
    ) {
        environments.Add(deployment.EnvironmentId.Sanitise(), environment);

        var deploy = deployment.IntoDomainOrDefault(
            environments,
            new Dictionary<string, Release>(),
            projects
        );

        deploy.Should().BeNull();
    }

    [Theory, AutoData]
    public void Deployment_IntoDomainOrDefault_With_No_Matching_Project_Returns_Null(
        Deployment deployment,
        Env environment,
        Release release,
        Dictionary<string, Env> environments,
        Dictionary<string, Release> releases
    ) {
        environments.Add(deployment.EnvironmentId.Sanitise(), environment);
        releases.Add(deployment.ReleaseId.Sanitise(), release);

        var deploy = deployment.IntoDomainOrDefault(
            environments,
            releases,
            new Dictionary<string, Project>()
        );

        deploy.Should().BeNull();
    }

    [Theory, AutoData]
    public void Deployment_IntoDomain_With_No_Matching_Environment_Throws(
        Deployment deployment,
        Dictionary<string, Release> releases,
        Dictionary<string, Project> projects
    ) {
        var act = () => deployment.IntoDomain(
            new Dictionary<string, Env>(),
            releases,
            projects
        );

        act.Should().ThrowExactly<NotFoundException>()
            .WithMessage($"No match found for {nameof(Env)} with identifier {deployment.EnvironmentId}");
    }

    [Theory, AutoData]
    public void Deployment_IntoDomain_With_No_Matching_Releases_Throws(
        Deployment deployment,
        Env environment,
        Dictionary<string, Env> environments,
        Dictionary<string, Project> projects
    ) {
        environments.Add(deployment.EnvironmentId.Sanitise(), environment);

        var act = () => deployment.IntoDomain(
            environments,
            new Dictionary<string, Release>(),
            projects
        );

        act.Should().ThrowExactly<NotFoundException>()
            .WithMessage($"No match found for {nameof(Release)} with identifier {deployment.ReleaseId}");
    }

    [Theory, AutoData]
    public void Deployment_IntoDomain_With_No_Matching_Project_Throws(
        Deployment deployment,
        Env environment,
        Release release,
        Dictionary<string, Env> environments,
        Dictionary<string, Release> releases
    ) {
        environments.Add(deployment.EnvironmentId.Sanitise(), environment);
        releases.Add(deployment.ReleaseId.Sanitise(), release);

        var act = () => deployment.IntoDomain(
            environments,
            releases,
            new Dictionary<string, Project>()
        );

        act.Should().ThrowExactly<NotFoundException>()
            .WithMessage($"No match found for {nameof(Project)} with identifier {release.ProjectId}");
    }
}
