using System;
using FluentAssertions;
using NUnit.Framework;
using DocumentsApi.V1.Helpers;
using Newtonsoft.Json.Linq;

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
            var s = JObject.Parse("{\"id\":\"" + $"this-is-my-id" + "\"}");
            var result = Base64UrlHelpers.EncodeToBase64Url(s);
            result.Should().BeEquivalentTo("ewogICJpZCI6ICJ0aGlzLWlzLW15LWlkIgp9");
        }

        [Test]
        public void DecodeReturnsExpectedString()
        {
            var s = "ewogICJpZCI6ICJ0aGlzLWlzLW15LWlkIgp9";
            var result = Base64UrlHelpers.DecodeFromBase64Url(s);
            result.Should().BeEquivalentTo(JObject.Parse("{\"id\":\"" + $"this-is-my-id" + "\"}"));
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
