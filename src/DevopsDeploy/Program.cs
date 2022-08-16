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

Console.WriteLine(deployments[0]);
Console.WriteLine(environments[0]);
Console.WriteLine(projects[0]);
Console.WriteLine(releases[0]);
