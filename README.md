# ๐ devops-deploy

DevOps Deploy `Release Retention` tool to keep your releases ๐๐จ fast & light!

> For more details please check out the [๐ explanation guide](docs/release-retention.md).

## ๐ฅฝ Prerequisites

- [dotnet 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```

## ๐  Build

```bash
dotnet build
```

## ๐โโ๏ธ Run

```bash
dotnet run --project ./src/DevopsDeploy/DevopsDeploy.csproj
```

## ๐งผ Lint

```bash
dotnet format
```

## ๐งช Test

```bash
dotnet test
```

## ๐ฅง Coverage

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

![alt text](img/coverage.png)

# Assumptions

- All json fields are required except from `Release.Version` & if there are missing required values an error occurs.
- Any unsupported environments or projects found, are skipped and logged.
- Assuming timestamps of release and deployment at are all of the same offset i.e UTC

# Improvements

- Use `System.Text.Json` only used `Newtonsoft` to showcase my wrapper libary [json-pact](https://github.com/xsv24/json-pact) as the `System.Text.Json` wrapper in the libary is not quite ready.
- Could pass in max deployment `n` as an argument or make an environment variable.
- Could pass in raw json or json file paths as an argument or make an environment variable.
- Could add support for dotnet `app.settings.json` environment json files.
- Could add a git hook to automatically run `dotnet format`.
- Use of pipeline `lint -> test -> static code analysis -> code coverage -> vunerabililty scan`
- Better code coverage tool such as Sonarcloud.
- Set up dependabot to help keep dependencies upto date.
- Possibly suggest data/json structure be switched from arrays -> map / dictionary as it would make look ups faster.
- If timestamps for a deployment of the same group fallback to `Version` number maybe? and keep the higher version?
- Testing could be improved with more variation in cases.
