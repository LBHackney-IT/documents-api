using DocumentsApi.V1.UseCase.Interfaces;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.Helpers;
using System.Collections.Generic;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using System;
using System.Linq;

namespace DocumentsApi.V1.UseCase
{
    public class GetClaimsByTargetIdUseCase : IGetClaimsByTargetIdUseCase
    {
        private readonly IDocumentsGateway _documentsGateway;

        public GetClaimsByTargetIdUseCase(IDocumentsGateway documentsGateway)
        {
            _documentsGateway = documentsGateway;
        }

        public PaginatedClaimResponse Execute(PaginatedClaimRequest request)
        {
            var claims = new List<Claim>();
            Boolean hasNextPage = false;

            if (request.Before != null && request.After != null)
            {
                throw new BadRequestException("Please only pass before of after");
            }

            if (request.Before == null && request.After == null)
            {
                claims = _documentsGateway.FindClaimsByTargetId(request.TargetId, request.Limit);
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

            if (claims.Count == 0)
            {
                throw new NotFoundException($"No claims have been found with the specified parameters");
            }

            if (claims.Any() && claims.Count == request.Limit + 1)
            {
                hasNextPage = true;
                claims.RemoveAt(claims.Count - 1);
            }

            var before = Base64UrlHelpers.EncodeToBase64Url(claims.First().Id.ToString());
            var after = Base64UrlHelpers.EncodeToBase64Url(claims.Last().Id.ToString());

            var claimsResponse = new List<ClaimResponse>();
            foreach (var claim in claims)
            {
                claimsResponse.Add(claim.ToResponse());
            }
            var result = ResponseFactory.ToPaginatedClaimResponse(claimsResponse, before, after, hasNextPage);
            return result;
        }
    }
}
