using DevopsDeploy.Models;
using Serilog;

namespace DevopsDeploy.Domain {
    public static class DomainMapper {
        public static DeploymentCollection IntoDeploymentCollection(this DeploymentData data, int maxDeployments = 2) {
            if (maxDeployments <= 0) {
                throw new ArgumentException($"'{nameof(maxDeployments)}' must be greater than zero.", nameof(maxDeployments));
            }

            Log.Information("Processing deployments...");

            var collection = new DeploymentCollection(maxDeployments);

            foreach (var deployment in data.Deployments) {
                var domain = deployment.IntoDomainOrDefault(data.Environments, data.Releases, data.Projects);

                if (domain is null) continue;

                collection.Add(domain);
            }

            return collection;
        }

        internal static DeploymentDomain? IntoDomainOrDefault(
            this Deployment deployment,
            IReadOnlyDictionary<string, Env> environments,
            IReadOnlyDictionary<string, Release> releases,
            IReadOnlyDictionary<string, Project> projects
        ) {
            try {
                return deployment.IntoDomain(environments, releases, projects);
            } catch (NotFoundException ex) {
                ex.Log();
                return null;
            }
        }

        internal static DeploymentDomain IntoDomain(
            this Deployment deployment,
            IReadOnlyDictionary<string, Env> environments,
            IReadOnlyDictionary<string, Release> releases,
            IReadOnlyDictionary<string, Project> projects
        ) {
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
}
