using System;
using System.Threading.Tasks;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase.Interfaces;

namespace DocumentsApi.V1.UseCase
{
    public class CreateUploadUrlUseCase : ICreateUploadUrlUseCase
    {
        private IS3Gateway _s3Gateway;
        private IDocumentsGateway _documentsGateway;

        public CreateUploadUrlUseCase(IS3Gateway s3Gateway, IDocumentsGateway documentsGateway)
        {
            _s3Gateway = s3Gateway;
            _documentsGateway = documentsGateway;
        }

        public async Task<S3UploadPolicy> Execute(Guid documentId)
        {
            var document = _documentsGateway.FindDocument(documentId);

            if (document == null)
            {
                throw new NotFoundException($"Cannot find document with ID {documentId}");
            }

            if (document.Uploaded)
            {
                throw new BadRequestException($"Document with ID {documentId} has already been uploaded.");
            }

            return await _s3Gateway.GenerateUploadPolicy(document).ConfigureAwait(true);
        }
    }
}
