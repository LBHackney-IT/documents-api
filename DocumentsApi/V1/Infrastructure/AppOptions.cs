using System;

namespace DocumentsApi.V1.Infrastructure
{
    public class AppOptions
    {
        public string DatabaseConnectionString { get; set; }
        public string DocumentsBucketName { get; set; }

        public static AppOptions FromEnv()
        {
            return new AppOptions
            {
                DatabaseConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING"),
                DocumentsBucketName = Environment.GetEnvironmentVariable("BUCKET_NAME")
            };
        }
    }
}
