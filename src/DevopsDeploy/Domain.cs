using System.Collections.Immutable;
using DevopsDeploy.Models;

namespace DevopsDeploy;

public record DeploymentDomain(
    string Id,
    ReleaseDomain Release,
    Env Environment,
    DateTime DeployedAt
);

public record ReleaseDomain(
    string Id,
    Project Project,
    string? Version,
    DateTime Created
);

public class DeploymentCollection : SortedDictionary<ProjectEnvKey, ImmutableList<DeploymentDomain>>
{

    public DeploymentCollection(IDictionary<ProjectEnvKey, ImmutableList<DeploymentDomain>> map) : base(map) { }

    public void Log()
    {
        Serilog.Log.Debug("Unique group count: {Count}", Count);
        // Filter out invalid values, group by project & environment, sort grouped deployments by date.
        foreach (var (key, value) in this)
        {
            var deploy = value.FirstOrDefault();

            if (deploy is null) continue;

            Serilog.Log.Information(
                "ReleaseId: '{ReleaseId}' ProjectId: '{ProjectId}' EnvironmentId: '{EnvironmentId}', Deployment: '{Deployment}' @ '{DeployedAt}'",
                key.ReleaseId,
                key.ProjectId,
                key.EnvironmentId,
                deploy.Id,
                deploy.DeployedAt
            );
            Serilog.Log.Debug(
                "Other deployments timestamps: {OtherDeploymentsDeployedAt}",
                string.Join(", ", value.Select(v => $"'{v.DeployedAt}'"))
            );
        }
    }
}

