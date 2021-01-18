using System.Threading.Tasks;
using DocumentsApi.V1.Domain;

namespace DocumentsApi.V1.Gateways.Interfaces
{
    public interface IS3Gateway
    {
        public Task<S3UploadPolicy> GenerateUploadPolicy(Document document);
        public Task<string> GetObjectContentType(string key);
    }
}
