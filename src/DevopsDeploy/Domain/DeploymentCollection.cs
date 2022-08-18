using System.Collections.Immutable;

namespace DevopsDeploy.Domain;

public class DeploymentCollection : SortedDictionary<ProjectEnvKey, ImmutableList<DeploymentDomain>>
{

    public DeploymentCollection(IDictionary<ProjectEnvKey, ImmutableList<DeploymentDomain>> map) : base(map) { }

    public void Log()
    {
        Serilog.Log.Debug("Unique group count: {Count}", Count);

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
