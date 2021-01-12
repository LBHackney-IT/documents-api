# 3. Use NodeServices to create post policies

Date: 2021-01-12

## Status

Accepted

## Context

We need to create a presigned Post Policy, to allow clients to upload specific files directly to S3.

- We cannot use PresignedUrl, as it uses PUT requests (not POST) and requires the Content Type of the file to be encoded into the signed policy, which does not work for our use case as at the time we create this URL, we don't know the content type of the document
- We _should_ be able to use `S3PostUploadSignedPolicy` from the .NET AWS SDK, but this class is not available in .NET Core, only in .NET Framework ([github issue](https://github.com/aws/aws-sdk-net/issues/1094)).
- We could create a separate node lambda just to create this signed post policy, as the necessary functionality is available in the javascript SDK
- .NET Core offers functionality to interface with a NodeJS process in `NodeServices`, which would allow us to run the JS SDK without a separate lambda (⚠️ this class is _obsolete_)
- We could manually create the policy and sign it ourselves (described [here](https://docs.aws.amazon.com/general/latest/gr/signature-v4-examples.html#signature-v4-examples-dotnet)).

## Decision

We have decided to use `NodeServices`, as a separate lambda would introduce the risks of failure and performance impacts of multiple internal HTTP requests. Manually signing would be architecturally cleaner, but with more risk of failure, more code to maintain ourselves and would be more time intensive to implement.

When this issue is solved in the .NET AWS SDK, we should remove this functionality and use C# directly.

## Consequences

This unlocks the functionality without too much effort—it introduces performance impact, but a relatively negligible amount.
