using DevopsDeploy.Models;

namespace DevopsDeploy.Domain;

public record ReleaseDomain(
    string Id,
    Project Project,
    string? Version,
    DateTime Created
);
