# Documents API

Documents API is a Platform API to securely and easily store and retrieve documents.

## Stack

-   .NET 6.0 as a web framework.
-   nUnit v3.12 as a test framework.

## What does it do?

[**🚀 Swagger**](https://app.swaggerhub.com/apis-docs/Hackney/documents-api/1.0.0) ([Edit it here](https://app.swaggerhub.com/apis/Hackney/documents-api/1.0.0))

## Dependencies

-   [S3 Lambda Function](#s3-lambda-function)

## Contributing

### Prerequisites

1. Install [Docker][docker-download] (>v20.10).
2. Clone this repository.
3. Open it in your IDE of your choice.

### 1. Set up envars

In order to run the API locally, you will first need to copy the environment variables in the [.env.example](.env.example) file and save them to a new file called `.env` in the root of the project (same place as `.env.example`). This file should not be tracked by git, as it has been added to the `.gitignore`, so please do check that this is the case.

You will also need to separately set up `LBHPACKAGESTOKEN` env var (used to access the [lbh-core](https://github.com/LBHackney-IT/lbh-core) library) in your terminal profile or in every terminal window. To do that for PowerShell you can use this command:

```
[System.Environment]::SetEnvironmentVariable('LBHPACKAGESTOKEN','<TOKEN VALUE GOES HERE>',[System.EnvironmentVariableTarget]::User)
```

and for Linux/Mac you can use this command

```
export LBHPACKAGESTOKEN=<TOKEN VALUE GOES HERE>
```

Please make sure that the `GET_CLAIMS_ALLOWED_GOOGLE_GROUPS` env var contains `e2e-testing` group for the tests to pass locally.

The values for all the env vars can be found in the Parameter Store, in `Document-Evidence-Store-Staging` and `Document-Evidence-Store-Production` AWS accounts.

### 2. Set up containers

Next step, to set up the local Documents API container, `cd` into the root of the project
(same place as the Makefile) and run `make serve-local`. This will set up the database container,
S3 proxy, run an automatic migration and stand up the API container. There are other Make recipes in the file;

```
# build the image and start the db, s3 proxy, migration and API containers
$ make serve-local

# build the images
$ make build-local

# start the db, s3 proxy migration and API containers
$ make start-local
```

-   The API will run on `http://localhost:3003`
-   The database will run on `http://localhost:3004`
-   The S3 proxy will run on `http://localhost:5555`

### 3. Testing

There are two ways to test the application:

1. Run the tests in the test container
2. Run them locally

The simplest and most reliable way is running the tests in the container. You can do this by running `make serve-test`,
which will build the images and run the containers. There are other Make recipes in the file;

```
# build the image and start the db, s3 proxy, migration and test containers
$ make serve-test

# build the images
$ make build-test

# start the db, s3 proxy, migration and test containers
$ make start-test
```

However, you might want to run the tests locally, in order to debug them through your IDE. The codebase is also set up to allow this.
All you need to do is to make sure that your `CONNECTION_STRING` envar in `.env` is the same as the one in `.env.example`.

Then, run `make start-local` and make sure to clean any local data that you might have added -- you can do this manually through a database application
like TablePlus or DataGrip.

Then you can run your tests by running `dotnet test` or through your IDE.

### Agreed Testing Approach

-   Use nUnit, FluentAssertions and Moq
-   Always follow a TDD approach
-   Tests should be independent of each other
-   Gateway tests should interact with a real test instance of the database
-   Test coverage should never go down
-   All use cases should be covered by E2E tests
-   Optimise when test run speed starts to hinder development
-   Unit tests and E2E tests should run in CI
-   Test database schemas should match up with production database schema
-   Have integration tests which test from the PostgreSQL database to API Gateway

### Dependencies

#### S3 Lambda Function

This application contains two lambda functions — an API, and a function which is triggered when objects are created in the S3 bucket, which can be found in `DocumentsApi/V1/S3EntryPoint.cs`.

To test the S3 Lambda function with the staging AWS account, follow these steps:

1. Install [AWS lambda test tool](e18ebff8-2a46-4ee3-8d27-c36706ac006f): `dotnet tool install -g Amazon.Lambda.TestTool-3.1`
2. Create a document in the staging S3 bucket with the key `e18ebff8-2a46-4ee3-8d27-c36706ac006f`
3. Create the equivalent record in your local database:
    ```shell script
    psql --database documents_api -f database/s3-test-seed.sql
    ```
4. Run the test:
    ```shell script
    bin/dotnet lambda-test-tool-3.1 --no-ui \
      --profile AWS_PROFILE_NAME \
      --path `pwd`/DocumentsApi \
      --function-handler DocumentsApi::DocumentsApi.S3EntryPoint::DocumentCreated \
      --payload `pwd`/DocumentsApi.Tests/Fixtures/s3-object-created-event.json \
      --region eu-west-2a
    ```

### Release process

We use a pull request workflow, where changes are made on a branch and approved by one or more other maintainers before the developer can merge into `master` branch.

![Circle CI Workflow Example](docs/circle_ci_workflow.png)

Then we have an automated six step deployment process, which runs in CircleCI.

1. Automated tests (nUnit) are run to ensure the release is of good quality.
2. The application is deployed to staging automatically.
3. We manually confirm a production deployment in the CircleCI workflow once we're happy with our changes in staging.
4. The application is deployed to production.

Our staging and production environments are hosted by AWS. We would deploy to production per each feature/config merged into `main` branch.

### Creating A PR

Before you commit or push your code, you will need to run:

```sh
 dotnet tool install dotnet-format --version 5.1.225507
```

Otherwise your PR will automatically fail the CircleCI checks. This will install the formatting tool for the repository. From thereon, you can run:
```sh
dotnet dotnet-format
```
to format your code.

To help with making changes to code easier to understand when being reviewed, we've added a PR template.
When a new PR is created on a repo that uses this API template, the PR template will automatically fill in the `Open a pull request` description textbox.
The PR author can edit and change the PR description using the template as a guide.

## Static Code Analysis

### Using [FxCop Analysers](https://www.nuget.org/packages/Microsoft.CodeAnalysis.FxCopAnalyzers)

FxCop runs code analysis when the Solution is built.

Both the API and Test projects have been set up to **treat all warnings from the code analysis as errors** and therefore, fail the build.

However, we can select which errors to suppress by setting the severity of the responsible rule to none, e.g `dotnet_analyzer_diagnostic.<Category-or-RuleId>.severity = none`, within the `.editorconfig` file.
Documentation on how to do this can be found [here](https://docs.microsoft.com/en-us/visualstudio/code-quality/use-roslyn-analyzers?view=vs-2019).

## Data Migrations

### A good data migration

-   Record failure logs
-   Automated
-   Reliable
-   As close to real time as possible
-   Observable monitoring in place
-   Should not affect any existing databases

## Contacts

### Active Maintainers

-   **Selwyn Preston**, Lead Developer at London Borough of Hackney (selwyn.preston@hackney.gov.uk)
-   **Mirela Georgieva**, Lead Developer at London Borough of Hackney (mirela.georgieva@hackney.gov.uk)
-   **Matt Keyworth**, Lead Developer at London Borough of Hackney (matthew.keyworth@hackney.gov.uk)

### Other Contacts

-   **Rashmi Shetty**, Product Owner at London Borough of Hackney (rashmi.shetty@hackney.gov.uk)

[docker-download]: https://www.docker.com/products/docker-desktop
[made-tech]: https://madetech.com/
[aws-cli]: https://aws.amazon.com/cli/

# License

[MIT](./LICENSE)
