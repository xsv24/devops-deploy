using DevopsDeploy.Domain;
using DevopsDeploy.Models;
using Serilog;

// Simple logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

(await DeploymentData.FromJsonFiles())
    .IntoDeploymentCollection(maxDeployments: 4)
    .Log();