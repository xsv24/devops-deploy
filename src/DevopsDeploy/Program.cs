using DevopsDeploy.Domain;
using DevopsDeploy.Models;
using Serilog;

// Simple logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

var data = DeploymentData.FromJsonFiles();
var projectEnvDeployments = data.IntoDeploymentCollection(maxDeployments: 4);

projectEnvDeployments.Log();