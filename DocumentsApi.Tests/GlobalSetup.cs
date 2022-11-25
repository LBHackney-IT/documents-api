using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using dotenv.net;
using NUnit.Framework;

[SetUpFixture]
public class GlobalSetup
{
    [OneTimeSetUp]
    [SuppressMessage("ReSharper", "CA1031")]
    public void SetUp()
    {
        try
        {
            DotEnv.Load();
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
        catch
        {
            Console.Write("Could not find .env file");
        }
    }
}
