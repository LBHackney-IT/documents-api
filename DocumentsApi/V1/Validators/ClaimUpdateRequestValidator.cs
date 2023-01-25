using FluentValidation;
using DocumentsApi.V1.Boundary.Request;

namespace DocumentsApi.V1.Validators
{
    public class ClaimUpdateRequestValidator : AbstractValidator<ClaimUpdateRequest>
    {
        public ClaimUpdateRequestValidator()
        {
            RuleFor(x => x)
                .Must(ReceiveAtLeastOneParameter)
                .WithMessage("Please provide at least one value to be updated");
        }

        private bool ReceiveAtLeastOneParameter(ClaimUpdateRequest request)
        {
            return !(request.GroupId == null && request.ValidUntil == null);
        }
    }
}
