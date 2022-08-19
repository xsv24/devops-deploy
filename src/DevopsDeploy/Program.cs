using DevopsDeploy.Domain;
using DevopsDeploy.Models;
using Serilog;

// Simple logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

var artifacts = (await DeploymentData.FromJsonFiles())
    .IntoDeploymentCollection(maxDeployments: 1)
    .DeploymentIdsToPersist();

Log.Information("DeploymentId's to persist: {Artifacts}", artifacts);
