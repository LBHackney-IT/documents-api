using System;
using DocumentsApi.V1.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace DocumentsApi.Tests.V1.Domain
{
    [TestFixture]
    public class DocumentTests
    {
        private Document _classUnderTest = new Document();

        [Test]
        public void UploadedIsFalseBeforeFileHasBeenUploaded()
        {
            _classUnderTest.Uploaded.Should().BeFalse();
        }

        [Test]
        public void UploadedIsTrueWhenFileIsAlreadyUploaded()
        {
            _classUnderTest.UploadedAt = DateTime.Now;

            _classUnderTest.Uploaded.Should().BeTrue();
        }
    }
}
