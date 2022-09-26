using System;
using FluentAssertions;
using NUnit.Framework;
using DocumentsApi.V1.Helpers;

namespace DocumentsApi.Tests.V1.Helpers
{
    [TestFixture]
    public class Base64UrlHelpersTests
    {
        [Test]
        public void StringToByteArrayCanReturnAByteArray()
        {
            var s = "Test";
            var result = Base64UrlHelpers.StringToByteArray(s);
            result.Should().BeOfType<byte[]>();
        }
    }
}
