using System.Collections.Immutable;
using DevopsDeploy;
using DevopsDeploy.Models;
using JsonPact;
using JsonPact.NewtonSoft;
using Environment = DevopsDeploy.Models.Environment;

var pact = JsonPacts.Default(JsonPactCase.Pascal).IntoJsonPact();

var deployments = pact.Deserialize<Deployment[]>(File.ReadAllText("./Json/Deployments.json"))
    ?? throw new ArgumentException($"Expected valid a list of {nameof(Deployment)}'s.");

var environments = pact.Deserialize<Environment[]>(File.ReadAllText("./Json/Environments.json"))
    ?? throw new ArgumentException($"Expected valid a list of {nameof(Environment)}'s.");

var projects = pact.Deserialize<Project[]>(File.ReadAllText("./Json/Projects.json"))
    ?? throw new ArgumentException($"Expected valid a list of {nameof(Project)}'s.");

var releases = pact.Deserialize<Release[]>(File.ReadAllText("./Json/Releases.json"))
    ?? throw new ArgumentException($"Expected valid a list of {nameof(Release)}'s.");

// Convert lists to maps to allow for look ups without iterating.
var projectMap = projects.ToDictionary(project => project.Id.Sanitise());
var envMap = environments.ToDictionary(environment => environment.Id.Sanitise());
var releaseMap = releases.ToDictionary(release => release.Id.Sanitise());

// Filter out invalid values, group by project & environment, sort grouped deployments by date.
var deploymentReleaseEnvs = deployments
    .Where(deployment => envMap.HasKey(deployment.EnvironmentId))
    .Where(deployment => releaseMap.HasKey(deployment.ReleaseId))
    .Where(deployment => projectMap.HasKey(releaseMap[deployment.ReleaseId].ProjectId))
    .GroupBy(deployment => new ProjectEnvKey(releaseMap[deployment.ReleaseId].ProjectId, deployment.EnvironmentId))
    .ToImmutableSortedDictionary(
        deployment => deployment.Key,
        deployment => deployment.OrderByDescending(d => d.DeployedAt).ToImmutableList()
    );

foreach (var (key, value) in deploymentReleaseEnvs) {
    Console.WriteLine($"'{projectMap[key.ProjectId].Name}' '{envMap[key.EnvironmentId].Name}' -> '{value.First().Id}' @ '{value.First().DeployedAt}'");
    Console.WriteLine(string.Join(", ", value.Select(v => v.DeployedAt)));
    Console.WriteLine();
}
