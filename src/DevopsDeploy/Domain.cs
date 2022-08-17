using System.Collections.Immutable;
using DevopsDeploy.Models;
using Environment = DevopsDeploy.Models.Environment;

namespace DevopsDeploy;

public record DeploymentDomain(
    string Id,
    ReleaseDomain Release,
    Environment Environment,
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
        // Filter out invalid values, group by project & environment, sort grouped deployments by date.
        foreach (var (key, value) in this)
        {
            var deploy = value.First();
            Serilog.Log.Information(
                "ProjectId: '{ProjectId}' EnvironmentId: '{EnvironmentId}', Deployment: '{Deployment}' @ '{DeployedAt}'",
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

