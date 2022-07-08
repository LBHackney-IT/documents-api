using System;

namespace DocumentsApi.Tests
{
    public static class ConnectionString
    {
        public static string TestDatabase()
        {
            return
                $"{Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? "Host=db;Port=5432;Database=documents_api;Username=postgres;Password=mypassword"}";
        }
    }
}
