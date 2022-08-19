namespace DevopsDeploy.Domain;
public class DeploymentCollection : Dictionary<ProjectEnvKey, SortedList<DateTime, DeploymentDomain>> {

    private readonly int _maxDeployments;

    public DeploymentCollection(int maxDeployments) {
        _maxDeployments = maxDeployments;
    }

    public void Add(DeploymentDomain deployment) {
        var key = deployment.IntoKey();
        var deployments = this.GetValueOrDefault(key);

        if (deployments is null) {
            AddNewGroup(key, deployment);
        } else {
            AddToExistingGroup(key, deployment, deployments);
        }
    }

    public IEnumerable<string> DeploymentIdsToPersist() => Values.SelectMany(
        deployments => deployments.Values.Select(deployment => deployment.Id)
    );

    private void AddNewGroup(ProjectEnvKey key, DeploymentDomain deployment) {
        var deployments = new SortedList<DateTime, DeploymentDomain> {
            { deployment.DeployedAt, deployment }
        };

        Add(key, deployments);
        LogItem("Add", key, deployment, deployments);
    }

    private void AddToExistingGroup(
        ProjectEnvKey key,
        DeploymentDomain deployment,
        SortedList<DateTime, DeploymentDomain> deployments
    ) {
        deployments.Add(deployment.DeployedAt, deployment);

        if (deployments.Count <= _maxDeployments) return;

        var deleted = deployments.Values[0];
        deployments.RemoveAt(0);

        Serilog.Log.Debug(
            "Swapping deployment '{DeletedDeploymentId}' for '{UpdatedDeploymentId}'",
            deleted.Id,
            deployment.Id
        );

        LogItem("Del", key, deployment, deployments);
    }

    private static void LogItem(
        string action,
        ProjectEnvKey key,
        DeploymentDomain deployment,
        SortedList<DateTime, DeploymentDomain> value
    ) {
        Serilog.Log.Information(
            @"[{Action}] DeploymentId: '{DeploymentId}', ReleaseId: '{ReleaseId}' ProjectId: '{ProjectId}' EnvironmentId: '{EnvironmentId}'",
            action,
            deployment.Id,
            key.ReleaseId,
            key.ProjectId,
            key.EnvironmentId
        );

        Serilog.Log.Information(
            "\t Deployments: {Deployments}",
            value.Select(deploy => deploy.Value.Id)
        );
    }
}
