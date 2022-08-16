namespace DevopsDeploy.Models;

/// <summary>
/// A deployment is the way a <see cref="Release"/> of a <see cref="Project"/> ends up in an <see cref="Environment"/>.
/// Every time a <see cref="Release"/> for a <see cref="Project"/> is sent to an <see cref="Environment"/>, a new deployment is created.
/// <br></br><br></br>
///
/// Deployments can be rolled out to different environments i.e for testing, sandbox and production.
/// </summary>
/// <param name="Id">Identifier for the Deployment</param>
/// <param name="ReleaseId">Identifier of a <see cref="Release"/>.</param>
/// <param name="EnvironmentId">Identifier of a <see cref="Environment"/>.</param>
/// <param name="DeployedAt">Timestamp of the deployment</param>
public record Deployment(
    string Id,
    string ReleaseId,
    string EnvironmentId,
    DateTime DeployedAt
);