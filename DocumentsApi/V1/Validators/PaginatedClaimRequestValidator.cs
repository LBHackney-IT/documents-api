using DocumentsApi.V1.Boundary.Request;
using FluentValidation;

namespace DocumentsApi.V1.Validators
{
    public class PaginatedClaimRequestValidator : AbstractValidator<PaginatedClaimRequest>
    {
        public PaginatedClaimRequestValidator()
        {
            RuleFor(x => x)
                .Must(ReceiveOnlyBeforeOrAfter)
                .WithName("Before/After")
                .WithMessage("Please provide either Before or After or none of them");
            RuleFor(x => x.TargetId).NotEmpty().NotNull();
        }
        
        private bool ReceiveOnlyBeforeOrAfter(PaginatedClaimRequest request)
        {
            return (request.Before == null || request.After == null);
        }
    }
}
