using DevopsDeploy.Models;

namespace DevopsDeploy.Domain {
    public record DeploymentDomain(
        string Id,
        ReleaseDomain Release,
        Env Environment,
        DateTime DeployedAt
    );
}