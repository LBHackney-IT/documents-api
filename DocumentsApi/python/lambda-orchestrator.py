import json
import boto3
import botocore

# Disable SSL certificate checking here because we need to retrieve the certificate from SSM
# Ignore any InsecureRequestWarning in the lambda logs
ssm_client = boto3.client('ssm', verify=False)

def lambda_handler(event, context):
   # Retrieve SSL certificate from SSM
   ssl_certificate = ssm_client.get_parameter(Name='palo-alto-ssl-certificate')
   # Write it to a file because 'verify' requires the certificate from the file system for our S3 client
   # We cannot simply pass it as a string
   ssl_certificate_file = '/tmp/palo-alto-ssl-certificate.crt'
   f = open(ssl_certificate_file, 'w')
   f.write(ssl_certificate['Parameter']['Value'])
   f.close()

   # Configure clients to use the SSL certificate we just wrote to
   s3_client = boto3.client('s3', verify=ssl_certificate_file)
   lambda_client = boto3.client("lambda", verify=ssl_certificate_file)

   # event contains all information about uploaded object
   print("Event :", event)

   # Bucket Name where file was uploaded
   bucket_name = event['Records'][0]['s3']['bucket']['name']

   # Filename of object (with path)
   file_key_name = event['Records'][0]['s3']['object']['key']

   # Copy Source Object
   copy_source_object = {'Bucket': bucket_name, 'Key': file_key_name}

   # If this is successful we can continue and copy the file
   try:
       # Get filename only and prepend with tmp as this is the only ephemeral storage lambdas have
       download_path = '/tmp/' + file_key_name.split('/')[-1]
       print("s3_client.download_file", bucket_name, file_key_name, download_path)
       s3_client.download_file(bucket_name, file_key_name, download_path)

   except Exception as e:
        print('An exception occurred: {}'.format(e))
        print("There was an error when attempting to download the file, moving to quarantine")

        quarantine_file_key_name = file_key_name.replace('pre-scan/', 'quarantine/')
        print("s3_client.copy_object", copy_source_object, bucket_name, quarantine_file_key_name)
        s3_client.copy_object(CopySource=copy_source_object, Bucket=bucket_name, Key=quarantine_file_key_name)

        print("Deleting the original file")
        print("s3_client.delete_object", bucket_name, file_key_name)
        s3_client.delete_object(Bucket=bucket_name, Key=file_key_name)
        return {
           'statusCode': 200,
           'body': json.dumps('Document Orchestrator finished successfully')
        }

   clean_file_key_name  = file_key_name.replace('pre-scan/', 'clean/')
   print("s3_client.copy_object", copy_source_object, bucket_name, clean_file_key_name)
   s3_client.copy_object(CopySource=copy_source_object, Bucket=bucket_name, Key=clean_file_key_name)

   print("Deleting the original file")
   print("s3_client.delete_object", bucket_name, file_key_name)
   s3_client.delete_object(Bucket=bucket_name, Key=file_key_name)

   print("Invoking documents-api-malware-scan-successful", clean_file_key_name)
   # Update the event object with the updated key now we've moved the file to the clean directory. Send this payload to the next lambda
   event['Records'][0]['s3']['object']['key'] = clean_file_key_name
   response = lambda_client.invoke(
       FunctionName = 'documents-api-malware-scan-successful',
       Payload = json.dumps(event)
   )
   print("Response of lambda_client.invoke", response)

   return {
       'statusCode': 200,
       'body': json.dumps('Document Orchestrator finished successfully')
   }
