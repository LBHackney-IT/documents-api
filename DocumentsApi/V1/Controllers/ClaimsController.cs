using System;
using System.ComponentModel.DataAnnotations;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Amazon.S3;

namespace DocumentsApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/claims")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class ClaimsController : BaseController
    {
        private ICreateClaimUseCase _createClaimUseCase;
        private readonly IFindClaimByIdUseCase _findClaimByIdUseCase;
        private readonly IUpdateClaimStateUseCase _updateClaimStateUseCase;
        private readonly ICreateClaimAndUploadDocumentUseCase _createClaimAndUploadDocumentUseCase;
        private readonly IGetClaimAndDocumentUseCase _getClaimAndDocumentUseCase;
        private readonly IGeneratePreSignedDownloadUrlUseCase _generatePreSignedDownloadUrlUseCase;

        public ClaimsController(
            ICreateClaimUseCase createClaimUseCase,
            IFindClaimByIdUseCase findClaimByIdUseCase,
            IUpdateClaimStateUseCase updateClaimStateUseCase,
            ICreateClaimAndUploadDocumentUseCase createClaimAndUploadDocumentUseCase,
            IGetClaimAndDocumentUseCase getClaimAndDocumentUseCase,
            IGeneratePreSignedDownloadUrlUseCase generatePreSignedDownloadUrlUseCase
        )
        {
            _createClaimUseCase = createClaimUseCase;
            _findClaimByIdUseCase = findClaimByIdUseCase;
            _updateClaimStateUseCase = updateClaimStateUseCase;
            _createClaimAndUploadDocumentUseCase = createClaimAndUploadDocumentUseCase;
            _getClaimAndDocumentUseCase = getClaimAndDocumentUseCase;
            _generatePreSignedDownloadUrlUseCase = generatePreSignedDownloadUrlUseCase;
        }

        /// <summary>
        /// Creates a new claim and document
        /// </summary>
        /// <response code="201">Saved</response>
        /// <response code="400">Request contains invalid parameters</response>
        /// <response code="401">Request lacks valid API token</response>
        [HttpPost]
        public IActionResult CreateClaim(ClaimRequest request)
        {
            try
            {
                var result = _createClaimUseCase.Execute(request);
                return Created(new Uri($"/evidence_requests/{result.Id}", UriKind.Relative), result);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.ValidationResponse.Errors);
            }
        }

        /// <summary>
        /// Creates a new claim and uploads a document
        /// </summary>
        /// <response code="201">Claim created and document uploaded</response>
        /// <response code="400">Request contains invalid parameters</response>
        /// <response code="400">A document has already been uploaded</response>
        /// <response code="401">Request lacks valid API token</response>
        /// <response code="404">Document not found</response>
        /// <response code="500">Document upload exception</response>
        [HttpPost]
        [Route("claim_and_document")]
        public IActionResult CreateClaimAndUploadDocument(ClaimAndUploadDocumentRequest request)
        {
            try
            {
                var result = _createClaimAndUploadDocumentUseCase.Execute(request);
                return Created(new Uri($"/claim_and_document/{result.ClaimId}", UriKind.Relative), result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (DocumentUploadException e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Get a claim
        /// </summary>
        /// <response code="200">Found</response>
        /// <response code="404">Claim not found</response>
        /// <response code="401">Request lacks valid API token</response>
        [HttpGet]
        [Route("{id}")]
        public IActionResult FindClaim([Required][FromRoute] Guid id)
        {
            try
            {
                var result = _findClaimByIdUseCase.Execute(id);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Gets a claim and a document
        /// </summary>
        /// <response code="200">Found</response>
        /// <response code="401">Request lacks valid API token</response>
        /// <response code="404">Claim not found</response>
        /// <response code="500">Amazon S3 exception</response>
        [HttpGet]
        [Route("claim_and_document/{claimId}")]
        public IActionResult GetClaimAndDocument([Required][FromRoute] Guid claimId)
        {
            try
            {
                var result = _getClaimAndDocumentUseCase.Execute(claimId);
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

        /// <summary>
        /// Updates the state of a claim
        /// </summary>
        /// <response code="200">Found</response>
        /// <response code="400">Request contains invalid parameters</response>
        /// <response code="401">Request lacks valid API token</response>
        /// <response code="404">Claim not found</response>
        [HttpPatch]
        [Route("{id}")]
        public IActionResult UpdateClaimState([Required][FromRoute] Guid id, [FromBody] ClaimUpdateRequest request)
        {
            try
            {
                var result = _updateClaimStateUseCase.Execute(id, request);
                return Ok(result);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Creates a download link for a document
        /// </summary>
        /// <response code="201">Saved</response>
        /// <response code="400">Request contains invalid parameters</response>
        /// <response code="401">Request lacks valid API token</response>
        [HttpGet]
        [Route("{claimId}/download_links")]
        public IActionResult GeneratePreSignedDownloadUrl([FromRoute] Guid claimId)
        {
            try
            {
                var result = _generatePreSignedDownloadUrlUseCase.Execute(claimId);
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
