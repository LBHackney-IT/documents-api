using System;
using Amazon.S3;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DocumentsApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/documents")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class DocumentsController : BaseController
    {
        private readonly IUploadDocumentUseCase _uploadDocumentUseCase;
        private readonly IDownloadDocumentUseCase _downloadDocumentUseCase;

        public DocumentsController(
            IUploadDocumentUseCase uploadDocumentUseCase,
            IDownloadDocumentUseCase downloadDocumentUseCase)
        {
            _uploadDocumentUseCase = uploadDocumentUseCase;
            _downloadDocumentUseCase = downloadDocumentUseCase;
        }

        [HttpPost]
        [Route("{id}")]
        public IActionResult UploadDocument([FromForm] DocumentUploadRequest request)
        {
            try
            {
                var httpStatusCode = _uploadDocumentUseCase.Execute(request);
                return StatusCode(int.Parse(httpStatusCode.ToString()));
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
        /// Retrieves the document in base64 representation
        /// </summary>
        /// <response code="200">Found</response>
        /// <response code="400">Request contains invalid parameters</response>
        /// <response code="401">Request lacks valid API token</response>
        /// <response code="500">Amazon S3 exception</response>
        [HttpGet]
        [Route("{documentId}")]
        [Produces("application/json")]
        public IActionResult GetDocument([FromRoute] Guid documentId)
        {
            try
            {
                var result = _downloadDocumentUseCase.Execute(documentId);
                return Ok(result);
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
