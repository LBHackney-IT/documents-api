using System.Threading.Tasks;
using DocumentsApi.V1.Domain;
using System.Diagnostics.CodeAnalysis;

namespace DocumentsApi.V1.Gateways.Interfaces
{
    public interface IS3Gateway
    {
        public Task<S3UploadPolicy> GenerateUploadPolicy(Document document);
        public Task<string> GetObjectContentType(string key);

        // Suppress CA1055 because GetPreSignedURL returns a string, not an Uri
        [SuppressMessage("ReSharper", "CA1055")]
        public string GeneratePreSignedDownloadUrl(Document document);
    }
}
