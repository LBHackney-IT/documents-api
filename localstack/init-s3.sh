#!/bin/bash
set -euo pipefail

bucket_name="${BUCKET_NAME:-testBucketName}"

if ! awslocal s3api head-bucket --bucket "$bucket_name" >/dev/null 2>&1; then
  awslocal s3 mb "s3://${bucket_name}"
fi

awslocal s3api put-bucket-cors \
  --bucket "$bucket_name" \
  --cors-configuration '{
    "CORSRules": [
      {
        "AllowedOrigins": ["*"],
        "AllowedMethods": ["GET", "POST", "PUT", "HEAD"],
        "AllowedHeaders": ["*"],
        "ExposeHeaders": ["ETag", "x-amz-request-id", "x-amz-id-2", "x-amz-server-side-encryption"],
        "MaxAgeSeconds": 3000
      }
    ]
  }'
