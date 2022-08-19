namespace DevopsDeploy.Domain;

/// <summary>
/// Composite key used to group projects and environments.
/// Tuple is used as it implements <see cref="IComparable"/> required to be used as a key.
/// </summary>
public class ProjectEnvKey : Tuple<string, string, string> {
    public string ReleaseId { get; init; }
    public string ProjectId { get; init; }
    public string EnvironmentId { get; init; }

    public ProjectEnvKey(string releaseId, string projectId, string environmentId) : base(releaseId.Sanitise(), projectId.Sanitise(), environmentId.Sanitise()) {
        ReleaseId = Item1;
        ProjectId = Item2;
        EnvironmentId = Item3;
    }
}
