using System;
using FluentAssertions;
using NUnit.Framework;
using DocumentsApi.V1.Boundary.Response.Exceptions;
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

        [Test]
        public void EncodeReturnsExpectedBase64URL()
        {
            var s = "Test";
            var result = Base64UrlHelpers.EncodeToBase64Url(s);
            result.Should().BeEquivalentTo("VGVzdA");
        }

        [Test]
        public void DecodeReturnsExpectedString()
        {
            var s = "VGVzdA";
            var result = Base64UrlHelpers.DecodeFromBase64Url(s);
            result.Should().BeEquivalentTo("Test");
        }
        [Test]
        public void DecodeThrowsExceptionToMalformedString()
        {
            var s = "VGVzdApppppp?";
            Action act = () => Base64UrlHelpers.DecodeFromBase64Url(s);
            act.Should().Throw<System.Exception>().WithMessage($"Illegal base64url string!");
        }

    }
}
