namespace DevopsDeploy.Models;

/// <summary>
/// Composite key use to group projects and environments.
/// Tuple is used as it implements <see cref="IComparable"/> required to be used as a key.
/// </summary>
public class ProjectEnvKey : Tuple<string, string> {
    public string ProjectId { get; init; }
    public string EnvironmentId { get; init; } 

    public ProjectEnvKey(string projectId, string environmentId) : base(projectId.Sanitise(), environmentId.Sanitise())
    {
        ProjectId = Item1;
        EnvironmentId = Item2;
    }
} 