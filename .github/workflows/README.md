# GitHub Actions Workflows

This page describes the CI/CD workflows for RestaurantOrderManager app using [GitHub Actions](https://github.com/mariuszsokolowski/RestaurantOrderManager).

## Workflow
### 1. 🔨 `dontet-build`

This workflow is responsible for manually building the .NET application (`data`, `api`, and `client`) on each push or pull request to the `master` branch. 

#### Workflow Details:
- **Trigger**: 
  - Runs on every push and pull request to the `master` branch.
- **Runs on**: `ubuntu-latest`
- **Steps**:
  1. Checks out the repository code using the `actions/checkout@v2`.
  2. Sets up the .NET SDK version `8.x` using the `actions/setup-dotnet@v1`.
  3. Builds each project (`data`, `api`, and `client`) individually by navigating to their respective directories and running `dotnet build` in Release configuration.

#### Workflow File: `dotnet-build.yml`