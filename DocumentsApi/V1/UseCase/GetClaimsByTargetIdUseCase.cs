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
using Newtonsoft.Json.Linq;

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
            Boolean hasBefore = false;
            Boolean hasAfter = false;

            if (request.Before == null && request.After == null)
            {
                claims = _documentsGateway.FindPaginatedClaimsByTargetId(request.TargetId, request.Limit + 1, null, null);
                if (claims.Count == request.Limit + 1)
                {
                    hasAfter = true;
                    claims.RemoveAt(claims.Count - 1);
                }
            }

            else if (request.After != null)
            {
                var decodedNextPageCursorId = DecodePaginationToken(request.After);
                claims = _documentsGateway.FindPaginatedClaimsByTargetId(request.TargetId, request.Limit + 1, decodedNextPageCursorId, isNextPage: true);
                hasBefore = true;

                if (claims.Count == request.Limit + 1)
                {
                    hasAfter = true;
                    claims.RemoveAt(claims.Count - 1);
                }
            }
            else
            {
                var decodedPreviousPageCursorId = DecodePaginationToken(request.Before);
                claims = _documentsGateway.FindPaginatedClaimsByTargetId(request.TargetId, request.Limit + 1, decodedPreviousPageCursorId, isNextPage: false);
                hasAfter = true;

                if (claims.Count == request.Limit + 1)
                {
                    hasBefore = true;
                    claims.RemoveAt(0);
                }
            }

            string before = "";
            string after = "";

            if (claims.Any())
            {
                var toBeEncodedBeforeCursor = JObject.Parse("{\"id\":\"" + $"{claims.First().Id.ToString()}" + "\"}");
                var toBeEncodedAfterCursor = JObject.Parse("{\"id\":\"" + $"{claims.Last().Id.ToString()}" + "\"}");
                before = Base64UrlHelpers.EncodeToBase64Url(toBeEncodedBeforeCursor);
                after = Base64UrlHelpers.EncodeToBase64Url(toBeEncodedAfterCursor);
            }

            var claimsResponse = new List<ClaimResponse>();
            foreach (var claim in claims)
            {
                claimsResponse.Add(claim.ToResponse());
            }
            var result = ResponseFactory.ToPaginatedClaimResponse(claimsResponse, before, after, hasBefore, hasAfter);
            return result;
        }

        private static Guid DecodePaginationToken(string token)
        {
            Guid decodedTokenCursorId;
            try
            {
                var parsedAfter = Base64UrlHelpers.DecodeFromBase64Url(token);
                decodedTokenCursorId = Guid.Parse((string) parsedAfter["id"]);
            }
            catch (Exception e)
            {
                throw new BadRequestException($"Error when trying to decode the pagination token: {e.Message}");
            }
            return decodedTokenCursorId;
        }
    }
}
