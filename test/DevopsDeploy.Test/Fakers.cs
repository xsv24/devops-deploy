using AutoFixture;
using DevopsDeploy.Models;

namespace DevopsDeploy.Test;

public static class Fake
{
    private static readonly Fixture Mocker = new();

    public static void AddLinkedDeployment(
        this List<Deployment> deployments,
        DateTime deployedAt,
        Env env,
        Release release
    )
    {
        var deployment = FakeDeployment() with
        {
            EnvironmentId = env.Id,
            ReleaseId = release.Id,
            DeployedAt = deployedAt
        };

        deployments.Add(deployment);
    }

    public static Dictionary<string, Project> FakeProjects(params string[] keys) =>
        keys.ToDictionary(key => key.Sanitise(), key => FakeProject() with { Id = key.Sanitise() });
    public static Dictionary<string, Env> FakeEnvs(params string[] keys) =>
        keys.ToDictionary(key => key.Sanitise(), key => FakeEnv() with { Id = key.Sanitise() });

    public static Dictionary<string, Release> FakeReleases(Project project, params string[] keys) =>
        keys.ToDictionary(key => key.Sanitise(), key => FakeRelease() with { Id = key.Sanitise(), ProjectId = project.Id.Sanitise() });

    public static string FakeStr() => Mocker.Create<string>().Sanitise();

    public static Release FakeRelease() => Mocker.Create<Release>() with
    {
        Id = FakeStr(),
        ProjectId = FakeStr()
    };

    public static Project FakeProject() => Mocker.Create<Project>() with
    {
        Id = FakeStr()
    };

    public static Env FakeEnv() => Mocker.Create<Env>() with
    {
        Id = FakeStr()
    };

    public static Deployment FakeDeployment() => Mocker.Create<Deployment>() with
    {
        Id = FakeStr(),
        ReleaseId = FakeStr(),
        EnvironmentId = FakeStr()
    };
}