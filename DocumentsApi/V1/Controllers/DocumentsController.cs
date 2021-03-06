using System;
using System.ComponentModel.DataAnnotations;
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


        /// <summary>
        /// Uploads the document to S3
        /// </summary>
        /// <response code="200">Uploaded</response>
        /// <response code="400">Request contains invalid parameters</response>
        /// <response code="401">Request lacks valid API token</response>
        /// <response code="500">Amazon S3 exception</response>
        /// <response code="500">Document upload exception</response>
        [HttpPost]
        [Route("{id}")]
        public IActionResult UploadDocument([FromRoute][Required] Guid id, [FromBody][Required] DocumentUploadRequest request)
        {
            try
            {
                _uploadDocumentUseCase.Execute(id, request);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (AmazonS3Exception e)
            {
                return StatusCode(500, e.Message);
            }
            catch (DocumentUploadException e)
            {
                return StatusCode(500, e.Message);
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
