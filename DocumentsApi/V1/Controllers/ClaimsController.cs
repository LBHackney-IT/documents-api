using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Amazon.S3;
using System.Threading.Tasks;

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
        private readonly IUpdateClaimUseCase _updateClaimUseCase;
        private readonly ICreateClaimAndS3UploadPolicyUseCase _createClaimAndS3UploadPolicyUseCase;
        private readonly IGetClaimAndPreSignedDownloadUrlUseCase _getClaimAndPreSignedDownloadUrlUseCase;
        private readonly IGeneratePreSignedDownloadUrlUseCase _generatePreSignedDownloadUrlUseCase;
        private readonly IGetClaimsByGroupIdUseCase _getClaimsByGroupIdUseCase;
        private readonly IUpdateClaimsGroupIdUseCase _updateClaimsGroupIdUseCase;

        public ClaimsController(
            ICreateClaimUseCase createClaimUseCase,
            IFindClaimByIdUseCase findClaimByIdUseCase,
            IUpdateClaimUseCase updateClaimUseCase,
            ICreateClaimAndS3UploadPolicyUseCase createClaimAndS3UploadPolicyUseCase,
            IGetClaimAndPreSignedDownloadUrlUseCase getClaimAndPreSignedDownloadUrlUseCase,
            IGeneratePreSignedDownloadUrlUseCase generatePreSignedDownloadUrlUseCase,
            IGetClaimsByGroupIdUseCase getClaimsByGroupIdUseCase,
            IUpdateClaimsGroupIdUseCase updateClaimsGroupIdUseCase
        )
        {
            _createClaimUseCase = createClaimUseCase;
            _findClaimByIdUseCase = findClaimByIdUseCase;
            _updateClaimUseCase = updateClaimUseCase;
            _createClaimAndS3UploadPolicyUseCase = createClaimAndS3UploadPolicyUseCase;
            _getClaimAndPreSignedDownloadUrlUseCase = getClaimAndPreSignedDownloadUrlUseCase;
            _generatePreSignedDownloadUrlUseCase = generatePreSignedDownloadUrlUseCase;
            _getClaimsByGroupIdUseCase = getClaimsByGroupIdUseCase;
            _updateClaimsGroupIdUseCase = updateClaimsGroupIdUseCase;
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
            var files = Directory.GetFiles("/opt", "*", SearchOption.AllDirectories);
            Console.WriteLine("OPT FILES:\n" + string.Join("\n", files));
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
        /// Creates a new claim and returns an upload policy from S3
        /// </summary>
        /// <response code="201">Claim created and upload policy returned</response>
        /// <response code="400">Request contains invalid parameters</response>
        /// <response code="401">Request lacks valid API token</response>
        [HttpPost]
        [Route("claim_and_upload_policy")]
        public async Task<IActionResult> CreateClaimAndUploadPolicy(ClaimRequest request)
        {
            try
            {
                var result = await _createClaimAndS3UploadPolicyUseCase.ExecuteAsync(request);
                return Created(new Uri($"/claim_and_upload_policy/{result.ClaimId}", UriKind.Relative), result);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
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
        /// Gets a claim and a presigned download url for the document
        /// </summary>
        /// <response code="200">Found</response>
        /// <response code="401">Request lacks valid API token</response>
        /// <response code="404">Claim not found</response>
        /// <response code="500">Amazon S3 exception</response>
        [HttpGet]
        [Route("claim_and_download_url/{claimId}")]
        public IActionResult GetClaimAndPresignedDownloadUrl([Required][FromRoute] Guid claimId)
        {
            try
            {
                var result = _getClaimAndPreSignedDownloadUrlUseCase.Execute(claimId);
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
        /// Updates a claim
        /// </summary>
        /// <response code="200">Found</response>
        /// <response code="400">Request contains invalid parameters</response>
        /// <response code="401">Request lacks valid API token</response>
        /// <response code="404">Claim not found</response>
        [HttpPatch]
        [Route("{id}")]
        public IActionResult UpdateClaim([Required][FromRoute] Guid id, [FromBody] ClaimUpdateRequest request)
        {
            try
            {
                var result = _updateClaimUseCase.Execute(id, request);
                return Ok(result);
            }
            catch (BadRequestException ex)
            {
                if (ex.ValidationResponse?.Errors == null || ex.ValidationResponse.Errors.Count == 0)
                {
                    return BadRequest(ex.Message);
                }
                return BadRequest(ex.ValidationResponse.Errors);
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

        /// <summary>
        /// Get claims by target ID
        /// </summary>
        /// <response code="200">Found</response>
        /// <response code="400">Request contains invalid parameters</response>
        /// <response code="401">Request lacks valid API token</response>
        [HttpGet]
        public IActionResult GetClaimsByGroupId([FromQuery] PaginatedClaimRequest request)
        {
            try
            {
                var result = _getClaimsByGroupIdUseCase.Execute(request);
                return Ok(result);
            }
            catch (BadRequestException ex)
            {
                if (ex.ValidationResponse?.Errors == null || ex.ValidationResponse.Errors.Count == 0)
                {
                    return BadRequest(ex.Message);
                }
                return BadRequest(ex.ValidationResponse.Errors);
            }
        }

        /// <summary>
        /// Update claims group ID
        /// </summary>
        /// <response code="200">Update successful</response>
        /// <response code="400">Request contains invalid parameters</response>
        /// <response code="401">Request lacks valid API token</response>
        [HttpPost]
        [Route("update")]
        public IActionResult UpdateClaimsGroupId([FromBody] ClaimsUpdateRequest request)
        {
            try
            {
                var result = _updateClaimsGroupIdUseCase.Execute(request);
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
    }
}
