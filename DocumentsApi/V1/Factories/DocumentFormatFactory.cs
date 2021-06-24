using System;
using System.IO;
using System.Text.RegularExpressions;
using Amazon.S3.Model;
using DocumentsApi.V1.Domain;

namespace DocumentsApi.V1.Factories
{
    public class DocumentFormatFactory : IDocumentFormatFactory
    {
        public string EncodeStreamToBase64(GetObjectResponse s3Response)
        {
            if (s3Response.ResponseStream != null)
            {
                using (Stream responseStream = s3Response.ResponseStream)
                {
                    byte[] bytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        responseStream.CopyTo(memoryStream);
                        bytes = memoryStream.ToArray();
                    }
                    return $"data:{s3Response.Headers.ContentType};base64," + Convert.ToBase64String(bytes);
                }
            }

            return string.Empty;
        }

        public Base64DecodedData DecodeBase64DocumentString(string documentString)
        {
            var base64Part = documentString.Split(",")[1];
            var fileTypePart = Regex.Match(documentString, @"(?<=:).+(?=;)").Value;

            return new Base64DecodedData()
            {
                DocumentBase64String = base64Part,
                DocumentType = fileTypePart
            };
        }
    }
}
