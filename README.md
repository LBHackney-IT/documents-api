# Documents API

Documents API is a Platform API to securely and easily store and retrieve documents.

## Stack

-   .NET Core as a web framework.
-   nUnit as a test framework.


## What does it do?

[**🚀 Swagger**](https://app.swaggerhub.com/apis-docs/Hackney/documents-api/1.0.0) ([Edit it here](https://app.swaggerhub.com/apis/Hackney/documents-api/1.0.0))


## Dependencies

-   S3 Mock API (described below)

## Contributing

### Setup

1. Install [Docker][docker-download].
2. Clone this repository.
4. Open it in your IDE.

### Development

To serve the application, run it using your IDE of choice, we use Visual Studio CE and JetBrains Rider on Mac.

The application can also be served locally using docker:
```sh
$ make build && make serve
```

#### Mock S3

To run this application, we need to contact S3 to create signed upload policies and download URLs. To be able to run this locally without setting up an AWS account, or to run the tests in isolation, we have used [S3Proxy](https://github.com/gaul/s3proxy).

Running the tests using `make` or `docker-compose` will handle this for you, but to run outside of those (e.g. with your IDE), you'll need to have the S3 proxy running in the background:

```shell script
docker-compose run -d s3-mock
```

You will also need to initialize your s3-mock which can be done by running the test _DocumentsApi.Tests/V1/E2ETests/S3LambdaTests.cs_

### S3 Lambda Function

This application contains two lambda functions—an API, and a function which is triggered when objects are created in the S3 bucket, which can be found in `DocumentsApi/V1/S3EntryPoint.cs`.

#### Test the S3 Lambda function with the Staging AWS Account

1. Install [AWS lambda test tool](e18ebff8-2a46-4ee3-8d27-c36706ac006f): `dotnet tool install -g Amazon.Lambda.TestTool-3.1`
2. Create a document in the staging S3 bucket with the key ``e18ebff8-2a46-4ee3-8d27-c36706ac006f``
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
4. The application is deployed to staging automatically.
5. We manually confirm a production deployment in the CircleCI workflow once we're happy with our changes in staging.
6. The application is deployed to production.

Our staging and production environments are hosted by AWS. We would deploy to production per each feature/config merged into `main` branch.

### Creating A PR

To help with making changes to code easier to understand when being reviewed, we've added a PR template.
When a new PR is created on a repo that uses this API template, the PR template will automatically fill in the `Open a pull request` description textbox.
The PR author can edit and change the PR description using the template as a guide.

## Static Code Analysis

### Using [FxCop Analysers](https://www.nuget.org/packages/Microsoft.CodeAnalysis.FxCopAnalyzers)

FxCop runs code analysis when the Solution is built.

Both the API and Test projects have been set up to **treat all warnings from the code analysis as errors** and therefore, fail the build.

However, we can select which errors to suppress by setting the severity of the responsible rule to none, e.g `dotnet_analyzer_diagnostic.<Category-or-RuleId>.severity = none`, within the `.editorconfig` file.
Documentation on how to do this can be found [here](https://docs.microsoft.com/en-us/visualstudio/code-quality/use-roslyn-analyzers?view=vs-2019).

## Testing

### Run the tests

```sh
$ make test
```

To run database tests locally (e.g. via Visual Studio) the `CONNECTION_STRING` environment variable will need to be populated with:

`Host=localhost;Database=testdb;Username=postgres;Password=mypassword"`

Note: The Host name needs to be the name of the stub database docker-compose service, in order to run tests via Docker.

If changes to the database schema are made then the docker image for the database will have to be removed and recreated. The restart-db make command will do this for you.

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