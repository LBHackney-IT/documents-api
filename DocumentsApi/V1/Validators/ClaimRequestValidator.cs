using System;
using DocumentsApi.V1.Boundary.Request;
using FluentValidation;

namespace DocumentsApi.V1.Validators
{
    public class ClaimRequestValidator : AbstractValidator<ClaimRequest>
    {
        public ClaimRequestValidator()
        {
            RuleFor(x => x.ServiceAreaCreatedBy).NotEmpty().NotNull();
            RuleFor(x => x.UserCreatedBy).NotEmpty().NotNull();
            RuleFor(x => x.ApiCreatedBy).NotEmpty().NotNull();
            RuleFor(x => x.RetentionExpiresAt).NotNull().GreaterThan(DateTime.UtcNow);
            RuleFor(x => x.DocumentName).MinimumLength(1).MaximumLength(300);
        }
    }
}
