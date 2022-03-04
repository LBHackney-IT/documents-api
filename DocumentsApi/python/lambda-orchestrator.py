import json
import boto3
import botocore

def lambda_handler(event, context):
   # Disable SSL for this instance of the client so that when we call 'download_file' then Palo Altos is able to scan the payload for malware
   s3_client_no_ssl = boto3.client('s3', use_ssl=False)
   # Enable SSL for other S3 operations
   s3_client = boto3.client('s3')
   lambda_client = boto3.client("lambda")

   # event contains all information about uploaded object
   print("Event :", event)

   # Bucket Name where file was uploaded
   bucket_name = event['Records'][0]['s3']['bucket']['name']

   # Filename of object (with path)
   file_key_name = event['Records'][0]['s3']['object']['key']

   # Copy Source Object
   copy_source_object = {'Bucket': bucket_name, 'Key': file_key_name}

   #Whitelist of accepted MIME types
   accepted_mime_types = [
    'application/msword', #.doc
    'application/pdf', #.pdf
    'application/vnd.apple.numbers', #.numbers
    'application/vnd.apple.pages', #.pages
    'application/vnd.ms-excel', #.xls
    'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', #.xlsx
    'application/vnd.openxmlformats-officedocument.wordprocessingml.document', #.docx
    'image/bmp', #.bmp
    'image/gif', #.gif
    'image/heic', #.heic
    'image/jpeg', #.jpeg or .jpg
    'image/png', #.png
    'text/plain', #.txt
    #'video/3gpp', #.3gpp or .3gp
    #'video/mp4', #.mp4
    #'video/quicktime', #.mov or .qt
   ] 

   #Check for MIME type of file
   head_object = s3_client.head_object(
       Bucket=bucket_name,
       Key=file_key_name,
   )
    
   if head_object['ContentType'] not in accepted_mime_types:
      print(f"File type {head_object['ContentType']} is not accepted!")
      print("Deleting the original file")
      print("s3_client.delete_object", bucket_name, file_key_name)
      s3_client.delete_object(Bucket=bucket_name, Key=file_key_name)
      return {
        'statusCode': 200,
        'body': json.dumps('Document Orchestrator finished successfully')
      }

   # If MIME type is accepted, we can continue and copy the file
   try:
       # Get filename only and prepend with tmp as this is the only ephemeral storage lambdas have
       download_path = '/tmp/' + file_key_name.split('/')[-1]
       print("s3_client.download_file", bucket_name, file_key_name, download_path)
       s3_client_no_ssl.download_file(bucket_name, file_key_name, download_path)

   except Exception as e:
        print('An exception occurred: {}'.format(e))
        print("There was an error when attempting to download the file, moving to quarantine")
        move_file_to_quarantine(file_key_name, copy_source_object, bucket_name, s3_client)

   move_file_to_clean_and_update_database_document_entity(event, lambda_client, file_key_name, copy_source_object, bucket_name, s3_client)
   

def move_file_to_quarantine(file_key_name, copy_source_object, bucket_name, s3_client):
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

def move_file_to_clean_and_update_database_document_entity(event, lambda_client, file_key_name, copy_source_object, bucket_name, s3_client):
    clean_file_key_name  = file_key_name.replace('pre-scan/', 'clean/')
    print("s3_client.copy_object", copy_source_object, bucket_name, clean_file_key_name)
    s3_client.copy_object(CopySource=copy_source_object, Bucket=bucket_name, Key=clean_file_key_name)

    print("Deleting the original file")
    print("s3_client.delete_object", bucket_name, file_key_name)
    s3_client.delete_object(Bucket=bucket_name, Key=file_key_name)

    print("Invoking documents-api-malware-scan-successful", clean_file_key_name)
    # Update the event object with the updated key now we've moved the file to the clean directory. 
    # Send this payload to the next lambda, which calls the UpdateUploadedDocumentUseCase that updates database with Size, UploadedAt and FileType
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
