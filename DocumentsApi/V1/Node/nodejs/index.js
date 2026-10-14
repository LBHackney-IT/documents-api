const { createPresignedPost } = require("@aws-sdk/s3-presigned-post");
const { S3Client } = require("@aws-sdk/client-s3");

/* TODO: Node being added to create signed Post Policies
   (see this issue: https://github.com/LBHackney-IT/documents-api/pull/6)
   Can be removed when presigned post policies are available in .NET
 */

module.exports = (callback, bucketName, key, expiry) => {
    try {
        const expiryInSeconds = parseInt(expiry);
        const clientConfig = {
            region: "eu-west-2",
            endpoint: process.env.S3_PUBLIC_ENDPOINT || process.env.S3_API_ENDPOINT,
        };
        if (process.env.S3_FORCE_PATH_STYLE === "true") {
            clientConfig.forcePathStyle = true;
        }
        const client = new S3Client(clientConfig);
        const data = createPresignedPost(client, {
            Bucket: bucketName,
            Key: key,
            Expires: expiryInSeconds,
            Fields: {
                acl: "private",
                key,
                "X-Amz-Server-Side-Encryption": "AES256",
            },
            Conditions: [
                { bucket: bucketName },
                { acl: "private" },
                { key },
                { "x-amz-server-side-encryption": "AES256" },
                ["content-length-range", 1, 50000000], // value is in bytes -> 50mb
                ["starts-with", "$Content-Type", ""],
            ],
        });

        data.then(result => callback(null, JSON.stringify(result)));
    } catch (err) {
        console.log("Failed generating pre-signed upload url", {
            error: err,
        });

        return callback(err);
    }
};
