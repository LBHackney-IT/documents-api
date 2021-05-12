import json
import boto3
import botocore

# boto3 S3 initialization
s3_client = boto3.client("s3", verify=False)

def lambda_handler(event, context):
   # event contains all information about uploaded object
   print("Event :", event)

   # Bucket Name where file was uploaded
   bucket_name = event['Records'][0]['s3']['bucket']['name']

   # Filename of object (with path)
   file_key_name = event['Records'][0]['s3']['object']['key']

   # Copy Source Object
   copy_source_object = {'Bucket': bucket_name, 'Key': file_key_name}

   # Get filename only and prepend with tmp as this is the only ephemeral storage lambdas have
   download_path = '/tmp/' + file_key_name.split('/')[-1]

   # If this is successful we can continue and copy the file
   try:
       print("s3_client.download_file", bucket_name, file_key_name, download_path)
       s3_client.download_file(bucket_name, file_key_name, download_path);

   except:
        print("There was an error when attempting to download the file, moving to quarantine")

        quarantine_file_key_name = file_key_name.replace('pre-scan/', 'quarantine/')
        print("s3_client.copy_object", copy_source_object, bucket_name, quarantine_file_key_name)
        s3_client.copy_object(CopySource=copy_source_object, Bucket=bucket_name, Key=quarantine_file_key_name)

        print("Deleting the originial file")
        print("s3_client.delete_object", bucket_name, file_key_name)
        s3_client.delete_object(Bucket=bucket_name, Key=file_key_name)
        return {
           'statusCode': 400,
           'body': json.dumps('Error!')
        }

   # S3 copy object operation
   clean_file_key_name  = file_key_name.replace('pre-scan/', 'clean/')
   print("s3_client.copy_object", copy_source_object, bucket_name, clean_file_key_name)
   s3_client.copy_object(CopySource=copy_source_object, Bucket=bucket_name, Key=clean_file_key_name)

   print("Deleting the originial file")
   print("s3_client.delete_object", bucket_name, file_key_name)
   s3_client.delete_object(Bucket=bucket_name, Key=file_key_name)
   # now we can delete the file in the pre_scan bucket as it has been copied to post_scan

   response = s3_client.invoke(
       FunctionName = 'arn:aws:lambda:eu-west-2:549011513230:function:documents-api-staging-s3'
   )
   print("s3_client.invoke", response)

   return {
       'statusCode': 200,
       'body': json.dumps('Hello from S3 events Lambda!')
   }
