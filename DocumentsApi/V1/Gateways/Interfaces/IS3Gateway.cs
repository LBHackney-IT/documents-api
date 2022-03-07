using System;
using System.Threading.Tasks;
using DocumentsApi.V1.Domain;
using Amazon.S3.Model;

namespace DocumentsApi.V1.Gateways.Interfaces
{
    public interface IS3Gateway
    {
        public Task<S3UploadPolicy> GenerateUploadPolicy();
        public string GeneratePreSignedDownloadUrl();
        public Task<string> GetObjectContentType(string key);
        public PutObjectResponse UploadDocument(Guid documentId, Base64DecodedData document);
        public GetObjectResponse GetObject(Document document);
    }
}
