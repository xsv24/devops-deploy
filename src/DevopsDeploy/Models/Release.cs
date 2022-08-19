namespace DevopsDeploy.Models {
    /// <summary>
    /// Snapshot in time of a project.
    /// </summary>
    /// <param name="Id">Unique identifier of the release</param>
    /// <param name="ProjectId">Identifier relating to a <see cref="Project"/>.</param>
    /// <param name="Version">Tag relating to a release (Optional).</param>
    /// <param name="Created">Timestamp of the release.</param>
    public record Release(
        string Id,
        string ProjectId,
        string? Version,
        DateTime Created
    );
}