using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Amazon.S3;

namespace DocumentsApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/documents")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class DocumentsController : BaseController
    {
        private readonly ICreateUploadPolicyUseCase _createUploadPolicyUseCase;
        private readonly IDownloadDocumentUseCase _downloadDocumentUseCase;

        public DocumentsController(ICreateUploadPolicyUseCase createUploadPolicyUseCase, IDownloadDocumentUseCase downloadDocumentUseCase)
        {
            _createUploadPolicyUseCase = createUploadPolicyUseCase;
            _downloadDocumentUseCase = downloadDocumentUseCase;
        }

        /// <summary>
        /// Creates a new claim and document
        /// </summary>
        /// <response code="201">Saved</response>
        /// <response code="400">Request contains invalid parameters</response>
        /// <response code="401">Request lacks valid API token</response>
        [HttpPost]
        [Route("{id}/upload_policies")]
        public async Task<IActionResult> CreateUploadPolicy([Required][FromRoute] Guid id)
        {
            try
            {
                var result = await _createUploadPolicyUseCase.Execute(id).ConfigureAwait(true);
                return Created(result.Url, result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Creates a download link for a document
        /// </summary>
        /// <response code="201">Saved</response>
        /// <response code="400">Request contains invalid parameters</response>
        /// <response code="401">Request lacks valid API token</response>
        [HttpGet]
        [Route("{documentId}")]
        public IActionResult GetDocument([FromRoute] Guid documentId)
        {
            try
            {
                var result = _downloadDocumentUseCase.Execute(documentId);
                return Created(new Uri($"/documents/{documentId}", UriKind.Relative), result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (AmazonS3Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
