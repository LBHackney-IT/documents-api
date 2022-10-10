using DocumentsApi.V1.UseCase.Interfaces;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.Helpers;
using DocumentsApi.V1.Validators;
using System.Collections.Generic;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace DocumentsApi.V1.UseCase
{
    public class GetClaimsByTargetIdUseCase : IGetClaimsByTargetIdUseCase
    {
        private readonly IDocumentsGateway _documentsGateway;
        private readonly ILogger<GetClaimsByTargetIdUseCase> _logger;

        public GetClaimsByTargetIdUseCase(IDocumentsGateway documentsGateway, ILogger<GetClaimsByTargetIdUseCase> logger)
        {
            _documentsGateway = documentsGateway;
            _logger = logger;
        }

        public PaginatedClaimResponse Execute(PaginatedClaimRequest request)
        {
            var validation = new PaginatedClaimRequestValidator().Validate(request);
            if (!validation.IsValid)
            {
                _logger.LogError("VALIDATION: {0}", validation.Errors.First().ErrorMessage);
                throw new BadRequestException(validation);
            }

            var claims = new List<Claim>();
            Boolean hasPreviousPage = false;
            Boolean hasNextPage = false;

            if (request.Before == null && request.After == null)
            {
                claims = _documentsGateway.FindPaginatedClaimsByTargetId(request.TargetId, request.Limit, null, null);
            }

            else if (request.After != null)
            {

                var parsedAfter = Guid.Parse(Base64UrlHelpers.DecodeFromBase64Url(request.After));
                claims = _documentsGateway.FindPaginatedClaimsByTargetId(request.TargetId, request.Limit, parsedAfter, isNextPage: true);
            }

            else
            {
                var parsedBefore = Guid.Parse(Base64UrlHelpers.DecodeFromBase64Url(request.Before));
                claims = _documentsGateway.FindPaginatedClaimsByTargetId(request.TargetId, request.Limit, parsedBefore, isNextPage: false);
            }

            if (request.Before != null)
            {
                hasNextPage = true;

                if (claims.Count == request.Limit + 1)
                {
                    hasPreviousPage = true;
                    claims.RemoveAt(0);
                }
            }
            else if (request.After != null)
            {
                hasPreviousPage = true;

                if (claims.Count == request.Limit + 1)
                {
                    hasNextPage = true;
                    claims.RemoveAt(claims.Count - 1);
                }
            }
            else
            {
                if (claims.Count == request.Limit + 1)
                {
                    hasNextPage = true;
                    claims.RemoveAt(claims.Count - 1);
                }
            }

            string before = "";
            string after = "";

            if (claims.Any())
            {
                before = Base64UrlHelpers.EncodeToBase64Url(claims.First().Id.ToString());
                after = Base64UrlHelpers.EncodeToBase64Url(claims.Last().Id.ToString());
            }

            var claimsResponse = new List<ClaimResponse>();
            foreach (var claim in claims)
            {
                claimsResponse.Add(claim.ToResponse());
            }
            var result = ResponseFactory.ToPaginatedClaimResponse(claimsResponse, before, after, hasPreviousPage, hasNextPage);
            return result;
        }
    }
}
