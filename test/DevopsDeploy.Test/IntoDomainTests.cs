using AutoFixture.Xunit2;
using DevopsDeploy.Models;
using FluentAssertions;

namespace DevopsDeploy.Test;

public class IntoDomainTests
{
    [Theory, AutoData]
    public void IntoDeploymentCollection_Does_Not_Error_On_Id_Miss_Matches(DeploymentData data)
    {
        data.IntoDeploymentCollection()
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
    )
    {
        environments.Add(deployment.EnvironmentId.Sanitise(), environment);
        releases.Add(deployment.ReleaseId.Sanitise(), release with { ProjectId = project.Id });
        projects.Add(project.Id.Sanitise(), project);

        var deploy = deployment.IntoDomainOrDefault(
            environments,
            releases,
            projects
        );

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
    )
    {
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
    )
    {
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
    )
    {
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
    )
    {
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
    )
    {
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
    )
    {
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