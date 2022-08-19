# 🚀 devops-deploy

DevOps Deploy `Release Retention` tool too keep your releases 🏎💨 fast & light!

> For more details please check out the [📖 explanation guide](docs/release-retention.md).

## 🥽 Prerequisites

- [dotnet 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```

## 🛠 Build

```bash
dotnet build
```

## 🏃‍♂️ Run

```bash
dotnet run --project ./src/DevopsDeploy/DevopsDeploy.csproj
```

## 🧼 Lint

```bash
dotnet format
```

## 🧪 Test

```bash
dotnet test
```

## 🥧 Coverage

```bash
dotnet test --collect:"XPlat Code Coverage"
```

Copy guid generated within `./test/DevopsDeploy/TestResults` and run the following to generate the code coverage report.

```bash
reportgenerator \
-reports:"./test/DevopsDeploy.Test/TestResults/*/coverage.cobertura.xml" \
-targetdir:"coverage" \
-reporttypes:Html
```

# Assumptions

- All json fields are required except from `Release.Version` if required values are not found error.
- Any unsupported environments or projects found, are skipped and logged.
- Assuming timestamps of release and deployment at are all of the same offset i.e UTC

# Improvements

- Could pass in max deployment `n` as an argument or make an environment variable.
- Could pass in json or json file paths as an argument or make an environment variable.
- Could add support for the dotnet `app.settings.json` environment json files.
- Could add a git hook to automatically run `dotnet format`.
- Use of pipeline `lint -> test -> static code analysis -> code coverage -> vunerabililty scan`
- Better code coverage tool such as Sonarcloud.
- Set up dependabot to help keep dependencies upto date.
- Possibly suggest data/json structure be switched from arrays -> map / dictionary as it would make look ups faster.
- If timestamps for a deployment of the same group fallback to `Version` number is at least incremental and the higher version will be used.
