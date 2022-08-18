using DevopsDeploy;
using Serilog;

// Simple logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

var data = DeploymentData.FromJsonFiles();
var projectEnvDeployments = data.IntoDeploymentCollection();

projectEnvDeployments.Log();