using System;
using System.ComponentModel.DataAnnotations;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

        public ClaimsController(
            ICreateClaimUseCase createClaimUseCase,
            IFindClaimByIdUseCase findClaimByIdUseCase,
            IUpdateClaimStateUseCase updateClaimStateUseCase
        )
        {
            _createClaimUseCase = createClaimUseCase;
            _findClaimByIdUseCase = findClaimByIdUseCase;
            _updateClaimStateUseCase = updateClaimStateUseCase;
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
    }
}
