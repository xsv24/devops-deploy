using DevopsDeploy.Models;
using JsonPact;
using JsonPact.NewtonSoft;
using Environment = DevopsDeploy.Models.Environment;

namespace DevopsDeploy;

public record DeploymentData(
    Deployment[] Deployments,
    IReadOnlyDictionary<string, Environment> Environments,
    IReadOnlyDictionary<string, Project> Projects,
    IReadOnlyDictionary<string, Release> Releases
)
{
    public static DeploymentData FromJsonFiles(string directory = "./Json", JsonOptions? options = null)
    {
        var pact = (options ?? JsonPacts.Default(JsonPactCase.Pascal)).IntoJsonPact();

        var deployments = pact.Deserialize<Deployment[]>(File.ReadAllText($"{directory}/Deployments.json"))
            ?? throw new ArgumentException($"Expected valid a list of {nameof(Deployment)}'s.");

        var environments = pact.Deserialize<Environment[]>(File.ReadAllText($"{directory}/Environments.json"))
            ?? throw new ArgumentException($"Expected valid a list of {nameof(Environment)}'s.");

        var projects = pact.Deserialize<Project[]>(File.ReadAllText($"{directory}/Projects.json"))
            ?? throw new ArgumentException($"Expected valid a list of {nameof(Project)}'s.");

        var releases = pact.Deserialize<Release[]>(File.ReadAllText($"{directory}/Releases.json"))
            ?? throw new ArgumentException($"Expected valid a list of {nameof(Release)}'s.");

        return new DeploymentData(
            deployments,
            // Convert lists to maps to allow for look ups without iterating.
            environments.ToDictionary(environment => environment.Id.Sanitise()),
            projects.ToDictionary(project => project.Id.Sanitise()),
            releases.ToDictionary(release => release.Id.Sanitise())
        );
    }
}