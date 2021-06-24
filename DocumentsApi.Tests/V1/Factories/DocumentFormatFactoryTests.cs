using System;
using System.IO;
using Amazon.S3.Model;
using Bogus;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Factories;
using NUnit.Framework;

namespace DocumentsApi.Tests.V1.Factories
{
    [TestFixture]
    public class DocumentFormatFactoryTests
    {
        private DocumentFormatFactory _documentFormatFactory;
        private Faker faker;

        [SetUp]
        public void SetUp()
        {
            _documentFormatFactory = new DocumentFormatFactory();
            faker = new Faker();
        }

        // Adapted from https://github.com/LBHackney-IT/mat-process-api/blob/master/mat-process-api.Tests/V1/Helper/ProcessImageDecoderTests.cs
        [TestCase("data:image/jpeg;base64,B0RXh8DNi8QcWJHJDgjRTkiVEc4Oisv/EABoBAAMBAQEBAAA")] //jpeg
        [TestCase("data:image/png;base64,B0RXh8DNi8QcWJHJDgjRTkiVEc4Oisv/EABoBAAMBAQEBAAA")] //png
        [TestCase("data:image/bmp;base64,B0RXh8DNi8QcWJHJDgjRTkiVEc4Oisv/EABoBAAMBAQEBAAA")] //bmp
        [TestCase("data:image/gif;base64,B0RXh8DNi8QcWJHJDgjRTkiVEc4Oisv/EABoBAAMBAQEBAAA")] //gif
        public void TestSuccessfulDecode(string base64String)
        {
            // Act
            var result = _documentFormatFactory.DecodeBase64DocumentString(base64String);

            // Assert
            Assert.IsInstanceOf<Base64DecodedData>(result);
        }

        // Adapted from https://github.com/LBHackney-IT/mat-process-api/blob/master/mat-process-api.Tests/V1/Factories/ImageRetrievalFactoryTest.cs
        [Test]
        public void TestEncodeStreamToBase64()
        {
            // Arrange
            var testResponse = new GetObjectResponse();
            testResponse.ResponseStream = new MemoryStream(faker.Random.Guid().ToByteArray());

            // Act
            var result = _documentFormatFactory.EncodeStreamToBase64(testResponse);
            var resultBase64 = result.Split(",")[1];

            // Assert
            Assert.IsNotEmpty(result);
            Assert.True(IsBase64String(resultBase64));
        }

        private static bool IsBase64String(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out int bytesParsed);
        }
    }
}
