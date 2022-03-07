using System;
using System.Threading.Tasks;
using DocumentsApi.V1.Domain;
using Amazon.S3.Model;
using System.Diagnostics.CodeAnalysis;

namespace DocumentsApi.V1.Gateways.Interfaces
{
    public interface IS3Gateway
    {
        public Task<S3UploadPolicy> GenerateUploadPolicy();
        // Suppress CA1055 because GetPreSignedURL returns a string, not an Uri
        [SuppressMessage("ReSharper", "CA1055")]
        public string GeneratePreSignedDownloadUrl();
        public Task<string> GetObjectContentType(string key);
        public PutObjectResponse UploadDocument(Guid documentId, Base64DecodedData document);
        public GetObjectResponse GetObject(Document document);
    }
}
