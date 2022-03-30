using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Boundary.Response;
using System.Threading.Tasks;

namespace DocumentsApi.V1.UseCase.Interfaces
{
    public interface ICreateClaimAndS3UploadPolicyUseCase
    {
        public Task<CreateClaimAndS3UploadPolicyResponse> ExecuteAsync(ClaimRequest request);
    }
}
