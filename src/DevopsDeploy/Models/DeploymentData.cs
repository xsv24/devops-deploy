using JsonPact;
using JsonPact.NewtonSoft;

namespace DevopsDeploy.Models;

public record DeploymentData(
    HashSet<Deployment> Deployments,
    IReadOnlyDictionary<string, Env> Environments,
    IReadOnlyDictionary<string, Project> Projects,
    IReadOnlyDictionary<string, Release> Releases
)
{
    public static async Task<DeploymentData> FromJsonFiles(string? directory = null, JsonOptions? options = null)
    {
        var path = directory ?? $"{Environment.CurrentDirectory}/Json";
        var pact = options ?? JsonPacts.Default(JsonPactCase.Pascal);

        // Asynchronously read each json separately file since we aren't worrying about dependencies. 
        // We use a 'HashSet' to avoid any possible duplicate data for iteration performance.
        var deployments = pact.DeserializeHashSetAsync<Deployment>($"{path}/Deployments.json");
        var environments = pact.DeserializeHashSetAsync<Env>($"{path}/Environments.json");
        var projects = pact.DeserializeHashSetAsync<Project>($"{path}/Projects.json");
        var releases = pact.DeserializeHashSetAsync<Release>($"{path}/Releases.json");

        await Task.WhenAll(deployments, environments, projects, releases);

        return new DeploymentData(
            deployments.Result,
            // Convert lists to maps take the hit on the iteration once to allow for look ups via key later.
            environments.Result.ToDictionary(environment => environment.Id.Sanitise()),
            projects.Result.ToDictionary(project => project.Id.Sanitise()),
            releases.Result.ToDictionary(release => release.Id.Sanitise())
        );
    }
}