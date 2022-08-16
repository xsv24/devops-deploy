using System.Text.Json;
using DevopsDeploy.Models;
using Environment = DevopsDeploy.Models.Environment;

var deployments = JsonSerializer.Deserialize<Deployment[]>(File.ReadAllText("./Json/Deployments.json"))
    ?? throw new ArgumentException($"Expected valid a list of {nameof(Deployment)}'s.");

var environments = JsonSerializer.Deserialize<Environment[]>(File.ReadAllText("./Json/Environments.json"))
    ?? throw new ArgumentException($"Expected valid a list of {nameof(Environment)}'s.");

var projects = JsonSerializer.Deserialize<Project[]>(File.ReadAllText("./Json/Projects.json"))
    ?? throw new ArgumentException($"Expected valid a list of {nameof(Project)}'s.");

var releases = JsonSerializer.Deserialize<Release[]>(File.ReadAllText("./Json/Releases.json"))
    ?? throw new ArgumentException($"Expected valid a list of {nameof(Release)}'s.");

Console.WriteLine(deployments[0]);
Console.WriteLine(environments[0]);
Console.WriteLine(projects[0]);
Console.WriteLine(releases[0]);
