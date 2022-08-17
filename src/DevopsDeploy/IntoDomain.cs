using System.Collections.Immutable;
using DevopsDeploy.Models;
using Environment = DevopsDeploy.Models.Environment;

namespace DevopsDeploy;

public static class DomainMapper
{

    public static DeploymentCollection IntoDeploymentCollection(this DeploymentData data)
    {
        var collection = data.Deployments
            .Select(deployment => deployment.IntoDomainOrDefault(data.Environments, data.Releases, data.Projects))
            .OfType<DeploymentDomain>()
            .GroupBy(deployment => new ProjectEnvKey(deployment.Release.Project.Id, deployment.Environment.Id))
            .ToImmutableSortedDictionary(
                deployment => deployment.Key,
                deployment => deployment.OrderByDescending(d => d.DeployedAt).ToImmutableList()
            );

        return new DeploymentCollection(collection);
    }

    public static DeploymentDomain? IntoDomainOrDefault(
        this Deployment deployment,
        IReadOnlyDictionary<string, Environment> environments,
        IReadOnlyDictionary<string, Release> releases,
        IReadOnlyDictionary<string, Project> projects
    )
    {
        try
        {
            return deployment.IntoDomain(environments, releases, projects);
        }
        catch (NotFoundException ex)
        {
            ex.Log();
            return null;
        }
    }

    public static DeploymentDomain IntoDomain(
        this Deployment deployment,
        IReadOnlyDictionary<string, Environment> environments,
        IReadOnlyDictionary<string, Release> releases,
        IReadOnlyDictionary<string, Project> projects
    )
    {
        var environment = environments.GetOrDefault(deployment.EnvironmentId)
                          ?? throw new NotFoundException(deployment.EnvironmentId, nameof(Environment));

        var release = releases.GetOrDefault(deployment.ReleaseId)
                      ?? throw new NotFoundException(deployment.ReleaseId, nameof(Release));

        var project = projects.GetOrDefault(release.ProjectId)
                      ?? throw new NotFoundException(release.ProjectId, nameof(Project));

        return new DeploymentDomain(
            Id: deployment.Id,
            DeployedAt: deployment.DeployedAt,
            Environment: environment,
            Release: release.IntoDomain(project)
        );
    }

    public static ReleaseDomain IntoDomain(
        this Release release,
        Project project
    ) => new(
        Id: release.Id,
        Version: release.Version,
        Created: release.Created,
        Project: project
    );
}