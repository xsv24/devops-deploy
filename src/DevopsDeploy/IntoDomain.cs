using System.Collections.Immutable;
using DevopsDeploy.Models;

namespace DevopsDeploy;

public static class DomainMapper
{
    public static DeploymentCollection IntoDeploymentCollection(this DeploymentData data, int maxDeployments = 2)
    {
        var collection = data.Deployments
            .Select(deployment => deployment.IntoDomainOrDefault(data.Environments, data.Releases, data.Projects))
            .OfType<DeploymentDomain>()
            .GroupBy(deployment => new ProjectEnvKey(
                deployment.Release.Id,
                deployment.Release.Project.Id,
                deployment.Environment.Id
            ))
            .ToImmutableSortedDictionary( // Only sorting for clarity here when debugging would remove for production.
                deployment => deployment.Key,
                deployment => deployment
                    .OrderByDescending(d => d.DeployedAt)
                    .Take(maxDeployments)
                    .ToImmutableList()
            );

        return new DeploymentCollection(collection);
    }

    internal static DeploymentDomain? IntoDomainOrDefault(
        this Deployment deployment,
        IReadOnlyDictionary<string, Env> environments,
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

    internal static DeploymentDomain IntoDomain(
        this Deployment deployment,
        IReadOnlyDictionary<string, Env> environments,
        IReadOnlyDictionary<string, Release> releases,
        IReadOnlyDictionary<string, Project> projects
    )
    {
        var environment = environments.GetOrDefault(deployment.EnvironmentId)
                          ?? throw new NotFoundException(deployment.EnvironmentId, nameof(Env));

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

    private static ReleaseDomain IntoDomain(
        this Release release,
        Project project
    ) => new(
        Id: release.Id,
        Version: release.Version,
        Created: release.Created,
        Project: project
    );
}