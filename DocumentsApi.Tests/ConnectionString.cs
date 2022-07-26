using System;

namespace DocumentsApi.Tests
{
    public static class ConnectionString
    {
        public static string TestDatabase()
        {
            return
                $"{Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? "Host=localhost;Port=3004;Database=documents_api;Username=postgres;Password=mypassword"}";
        }
    }
}
