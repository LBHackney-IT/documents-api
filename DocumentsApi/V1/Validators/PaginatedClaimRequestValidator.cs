using DocumentsApi.V1.Boundary.Request;
using FluentValidation;

namespace DocumentsApi.V1.Validators
{
    public class PaginatedClaimRequestValidator : AbstractValidator<PaginatedClaimRequest>
    {
        public PaginatedClaimRequestValidator()
        {
            RuleFor(x => x.Before).Null().Unless(x => x.After == null).WithMessage("Some error message");
            RuleFor(x => x.TargetId).NotEmpty().NotNull();
        }
    }
}