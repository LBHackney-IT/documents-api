using System;
using FluentValidation.Results;

namespace DocumentsApi.V1.Boundary.Response.Exceptions
{
    public class BadRequestException : Exception
    {
        public ValidationResult ValidationResponse { get; set; }

        public BadRequestException(ValidationResult validationResponse)
        {
            ValidationResponse = validationResponse;
        }
    }
}
