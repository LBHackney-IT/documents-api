# 2. Use s3 Presigned URLs for file uploading

Date: 2021-01-08

## Status

Accepted

## Context

There are a number of ways an API could allow clients to upload files to S3, the popular ones:
- Allow the API to accept Base 64 encoded files in a JSON POST request and subsequently send this blob to S3
- Allow the API to accept multipart form uploads, compile the parts on the server then send the file to S3
- Use the S3 Presigned URL functionality, which allows the client to act as the IAM which created the URL for a single operation, and upload the file directly to S3 themselves

## Decision

We decided to use the third option (presigned URLs), for the following reasons:
- sending the file via our own servers (options 1 & 2) create risks of losing data between the client and the destination
- it also incurs risk of privacy violation, as personal data have passed through our servers on their way to S3
- using multipart form uploads (option 2) breaks the JSON API format of our API and creates an inconsistent experience for our clients
- sending a file directly to S3, skipping intermediate servers, is the quickest option for the user's experience

## Consequences

Our clients will require slightly more logic in order to first create a presigned upload URL before uploading the file, and have to deal with extra unhappy cases (e.g. the presigned URL expiring) but this is more performant and more secure overall.
