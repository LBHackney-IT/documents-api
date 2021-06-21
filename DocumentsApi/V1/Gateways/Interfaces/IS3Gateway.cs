using System.Threading.Tasks;
using DocumentsApi.V1.Domain;
using Amazon.S3.Model;
using DocumentsApi.V1.Boundary.Request;

namespace DocumentsApi.V1.Gateways.Interfaces
{
    public interface IS3Gateway
    {
        public Task<string> GetObjectContentType(string key);
        public PutObjectResponse UploadDocument(DocumentUploadRequest documentUploadRequest);
        public GetObjectResponse GetObject(Document document);
    }
}
