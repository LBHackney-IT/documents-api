using System;

namespace DocumentsApi.V1.Infrastructure
{
    public static class AppOptions
    {
        public static string DatabaseConnectionString => Environment.GetEnvironmentVariable("CONNECTION_STRING");
    }
}
