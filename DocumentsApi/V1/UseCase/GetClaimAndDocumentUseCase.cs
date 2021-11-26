using System;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.Logging;

namespace DocumentsApi.V1.UseCase
{
    public class GetClaimAndDocumentUseCase : IGetClaimAndDocumentUseCase
    {
        private readonly IFindClaimByIdUseCase _findClaimByIdUseCase;
        private readonly IDownloadDocumentUseCase _downloadDocumentUseCase;
        private readonly ILogger<GetClaimAndDocumentUseCase> _logger;

        public GetClaimAndDocumentUseCase(
            IFindClaimByIdUseCase findClaimByIdUseCase,
            IDownloadDocumentUseCase downloadDocumentUseCase,
            ILogger<GetClaimAndDocumentUseCase> logger)
        {
            _findClaimByIdUseCase = findClaimByIdUseCase;
            _downloadDocumentUseCase = downloadDocumentUseCase;
            _logger = logger;
        }

        public ClaimAndDocumentResponse Execute(Guid claimId)
        {
            var claim = _findClaimByIdUseCase.Execute(claimId);
            var document = _downloadDocumentUseCase.Execute(claim.Document.Id);

            return claim.ToClaimAndDocumentResponse(document);
        }
    }
}
