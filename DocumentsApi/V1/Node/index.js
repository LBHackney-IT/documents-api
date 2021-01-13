const { createPresignedPost } = require("@aws-sdk/s3-presigned-post");
const { S3Client } = require("@aws-sdk/client-s3");

/* TODO: Node being added to create signed Post Policies
   (see this issue: https://github.com/LBHackney-IT/documents-api/pull/6)
   Can be removed when presigned post policies are available in .NET
 */
module.exports = async function (callback, bucketName, key, expiry) {
    try {
        const client = new S3Client({ region: "eu-west-2", endpoint: process.env.S3_API_ENDPOINT });
        const data = await createPresignedPost(client, {
            Bucket: bucketName,
            Key: key,
            Expires: expiry,
            Fields: {
                acl: 'private',
                key,
                'X-Amz-Server-Side-Encryption': 'AES256'
            },
            Conditions: [
                { bucket: bucketName },
                { acl: "private" },
                { key },
                { 'x-amz-server-side-encryption': 'AES256' },
                ['content-length-range', 1, 500000000], // 500mb
                ['starts-with', '$Content-Type', '']
            ],
        })

        return callback(null, JSON.stringify(data));
    } catch (err) {
        console.log('Failed generating pre-signed upload url', {
            error: err,
        });

        return callback(err);
    }
}
