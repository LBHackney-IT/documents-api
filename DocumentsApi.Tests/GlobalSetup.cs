using System;
using System.IO;
using dotenv.net;
using NUnit.Framework;

namespace DocumentsApi.Tests
{
    [SetUpFixture]
    public class GlobalSetup
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            DotEnv.Config(true, Path.GetFullPath("../../../../.env.example"));
        }
    }
}
