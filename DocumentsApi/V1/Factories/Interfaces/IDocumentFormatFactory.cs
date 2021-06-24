using Amazon.S3.Model;
using DocumentsApi.V1.Domain;

namespace DocumentsApi.V1.Factories
{
    public interface IDocumentFormatFactory
    {
        string EncodeStreamToBase64(GetObjectResponse s3Response);
        Base64DecodedData DecodeBase64DocumentString(string documentString);
    }
}
