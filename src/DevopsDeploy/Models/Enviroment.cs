namespace DevopsDeploy.Models;

/// <summary>
/// Collection of the things that get deployed to.
/// It would usually be composed of the physical and virtual machines where the project will run.
/// </summary>
/// <param name="Id">Identifier of the environment.</param>
/// <param name="Name">Display name of the environment.</param>
public record Environment(
    string Id,
    string Name
);