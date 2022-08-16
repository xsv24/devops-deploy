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
-reports:"./test/DevopsDeploy.Test/TestResults/{guid}/coverage.cobertura.xml" \
-targetdir:"coveragereport" \
-reporttypes:Html
```

# Improvements

- Better code coverage tool such as Sonarcloud.