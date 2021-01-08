using System;

namespace DocumentsApi.V1.Boundary.Response.Exceptions
{
    public class NotFoundException : Exception
    {

        public NotFoundException(string message) : base(message) { }
    }
}
