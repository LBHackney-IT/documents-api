using System;
using System.ComponentModel.DataAnnotations;
using Amazon.S3;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DocumentsApi.V1.Gateways.Interfaces;

namespace DocumentsApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/documents")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class DocumentsController : BaseController
    {
        private readonly ICreateUploadPolicyUseCase _createUploadPolicyUseCase;
        private readonly IS3Gateway _s3Gateway;

        public DocumentsController(
            ICreateUploadPolicyUseCase createUploadPolicyUseCase,
            IS3Gateway s3Gateway)
        {
            _createUploadPolicyUseCase = createUploadPolicyUseCase;
            _s3Gateway = s3Gateway;
        }

        /// <summary>
        /// Test endpoint
        /// </summary>
        /// <response code="201">Saved</response>
        /// <response code="400">Request contains invalid parameters</response>
        /// <response code="401">Request lacks valid API token</response>
        [HttpGet]
        [Route("{id}/upload_policies")]
        public async Task<IActionResult> CreateUploadPolicy([FromRoute][Required] Guid id)
        {
            try
            {
                var result = await _createUploadPolicyUseCase.Execute(id).ConfigureAwait(true);
                return Ok(result);
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
    }
}
