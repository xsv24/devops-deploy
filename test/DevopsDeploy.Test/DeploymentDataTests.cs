using DevopsDeploy.Models;
using FluentAssertions;

namespace DevopsDeploy.Test;

public class DeploymentDataTests
{
    [Fact]
    public async Task FromJsonFiles_With_Default_FilePath_Should_Load_DeploymentData()
    {
        Func<Task<DeploymentData>> act = async () => await DeploymentData.FromJsonFiles();

        var data = (await act.Should().NotThrowAsync()).Subject;

        data.Deployments.Should().HaveCount(10);
        data.Deployments
            .Select(deploy => deploy.Id)
            .Should()
            .BeEquivalentTo(Enumerable.Range(1, 10).Select(i => $"Deployment-{i}"));

        data.Releases.Should().HaveCount(8);
        data.Releases
            .Select(release => release.Value.Id)
            .Should()
            .BeEquivalentTo(Enumerable.Range(1, 8).Select(i => $"Release-{i}"));

        data.Projects.Should().HaveCount(2);
        data.Projects
            .Select(project => project.Value.Id)
            .Should()
            .BeEquivalentTo(Enumerable.Range(1, 2).Select(i => $"Project-{i}"));

        data.Environments.Should().HaveCount(2);
        data.Environments
            .Select(env => env.Value.Id)
            .Should()
            .BeEquivalentTo(Enumerable.Range(1, 2).Select(i => $"Environment-{i}"));
    }

    [Fact]
    public async Task FromJsonFiles_With_Valid_Custom_File_Path_Should_Not_Error()
    {
        Func<Task> act = async () => await DeploymentData.FromJsonFiles($"{Environment.CurrentDirectory}/TestData");
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task FromJsonFiles_With_Invalid_File_Path_Should_Error()
    {
        Func<Task> act = async () => await DeploymentData.FromJsonFiles("/junk");
        await act.Should().ThrowAsync<DirectoryNotFoundException>();
    }
}