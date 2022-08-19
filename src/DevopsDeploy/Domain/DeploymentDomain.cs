using DevopsDeploy.Models;

namespace DevopsDeploy.Domain {
    public record DeploymentDomain(
        string Id,
        ReleaseDomain Release,
        Env Environment,
        DateTime DeployedAt
    ) {
        public ProjectEnvKey IntoKey() => new(
            Release.Id.Sanitise(),
            Release.Project.Id.Sanitise(),
            Environment.Id.Sanitise()
        );
    }
}
