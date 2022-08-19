namespace DevopsDeploy.Models;
/// <summary>
/// A project represents the thing that is getting deployed
/// and contains all of the information about how it is deployed.
/// A project usually maps to the software that the team is deploying:
/// an application, a website, a database, or a service.
/// </summary>
/// <param name="Id">Identifier of the project.</param>
/// <param name="Name">Display name of the project.</param>
public record Project(
    string Id,
    string Name
);
