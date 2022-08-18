using DevopsDeploy.Models;
using JsonPact;
using JsonPact.NewtonSoft;

namespace DevopsDeploy;

public record DeploymentData(
    HashSet<Deployment> Deployments,
    IReadOnlyDictionary<string, Env> Environments,
    IReadOnlyDictionary<string, Project> Projects,
    IReadOnlyDictionary<string, Release> Releases
)
{
    public static DeploymentData FromJsonFiles(string? directory = null, JsonOptions? options = null)
    {
        var path = directory ?? $"{Environment.CurrentDirectory}/Json";
        
        var pact = (options ?? JsonPacts.Default(JsonPactCase.Pascal)).IntoJsonPact();

        // We use a 'HashSet' to avoid any possible duplicate data for iteration performance.
        var deployments = pact.Deserialize<HashSet<Deployment>>(File.ReadAllText($"{path}/Deployments.json"))
            ?? throw new ArgumentException($"Expected valid a list of {nameof(Deployment)}'s.");

        var environments = pact.Deserialize<HashSet<Env>>(File.ReadAllText($"{path}/Environments.json"))
            ?? throw new ArgumentException($"Expected valid a list of {nameof(Env)}'s.");

        var projects = pact.Deserialize<HashSet<Project>>(File.ReadAllText($"{path}/Projects.json"))
            ?? throw new ArgumentException($"Expected valid a list of {nameof(Project)}'s.");

        var releases = pact.Deserialize<HashSet<Release>>(File.ReadAllText($"{path}/Releases.json"))
            ?? throw new ArgumentException($"Expected valid a list of {nameof(Release)}'s.");

        return new DeploymentData(
            deployments,
            // Convert lists to maps take the hit on the iteration once to allow for look ups via key later.
            environments.ToDictionary(environment => environment.Id.Sanitise()),
            projects.ToDictionary(project => project.Id.Sanitise()),
            releases.ToDictionary(release => release.Id.Sanitise())
        );
    }
}