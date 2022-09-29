using System;
using AutoFixture;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Validators;
using FluentAssertions;
using NUnit.Framework;

namespace DocumentsApi.Tests.V1.Validators
{
    [TestFixture]
    public class ClaimRequestValidatorTests
    {
        private Fixture _fixture = new Fixture();
        private ClaimRequestValidator _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _classUnderTest = new ClaimRequestValidator();
        }

        [TestCase("")]
        [TestCase(null)]
        public void FailsWithInvalidUserCreatedBy(string value)
        {
            var request = _fixture.Build<ClaimRequest>()
                .With(x => x.UserCreatedBy, value)
                .Create();

            _classUnderTest.Validate(request).IsValid.Should().BeFalse();
        }

        [TestCase("")]
        [TestCase(null)]
        public void FailsWithInvalidApiCreatedBy(string value)
        {
            var request = _fixture.Build<ClaimRequest>()
                .With(x => x.ApiCreatedBy, value)
                .Create();

            _classUnderTest.Validate(request).IsValid.Should().BeFalse();
        }

        [TestCase("")]
        [TestCase(null)]
        public void FailsWithInvalidServiceAreaCreatedBy(string value)
        {
            var request = _fixture.Build<ClaimRequest>()
                .With(x => x.ServiceAreaCreatedBy, value)
                .Create();

            _classUnderTest.Validate(request).IsValid.Should().BeFalse();
        }

        [Test]
        public void FailsWithNullRetentionDate()
        {
            var request = _fixture.Build<ClaimRequest>()
                .Without(x => x.RetentionExpiresAt)
                .Create();

            _classUnderTest.Validate(request).IsValid.Should().BeFalse();
        }

        [Test]
        public void FailsWithInvalidRetentionDateInPast()
        {
            var request = _fixture.Build<ClaimRequest>()
                .With(x => x.RetentionExpiresAt, DateTime.UtcNow.AddDays(-1))
                .Create();

            _classUnderTest.Validate(request).IsValid.Should().BeFalse();
        }

        [Test]
        public void FailsWithDocumentNameLongerThan300Characters()
        {
            var request = _fixture.Build<ClaimRequest>()
                .With(x => x.DocumentName, "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec p")
                .Create();

            _classUnderTest.Validate(request).IsValid.Should().BeFalse();
        }

        [Test]
        public void FailsWithDocumentNameShorterThan1Character()
        {
            var request = _fixture.Build<ClaimRequest>()
                .With(x => x.DocumentName, "")
                .Create();

            _classUnderTest.Validate(request).IsValid.Should().BeFalse();
        }

        [Test]
        public void ValidatesAValidRequest()
        {
            var request = _fixture.Build<ClaimRequest>()
                .With(x => x.RetentionExpiresAt, DateTime.UtcNow.AddDays(1))
                .Create();

            _classUnderTest.Validate(request).IsValid.Should().BeTrue();
        }
        [Test]
        public void FailsWhenDocumentDescriptionLongerThan1000Characters()
        {
            var tooLongDescription = new string('T', 1001);
            var request = _fixture.Build<ClaimRequest>()
                .With(x => x.DocumentDescription, tooLongDescription)
                .Create();

            _classUnderTest.Validate(request).IsValid.Should().BeFalse();
        }

        [Test]
        public void AcceptsWhenDocumentDescriptionIsExactly1000Characters()
        {
            var validDescription = new string('T', 1000);
            var request = _fixture.Build<ClaimRequest>()
                .With(x => x.DocumentDescription, validDescription)
                .Create();

            _classUnderTest.Validate(request).IsValid.Should().BeTrue();
        }

        [Test]
        public void FailsWhenDocumentDescriptionShorterThan1Character()
        {
            var request = _fixture.Build<ClaimRequest>()
                .With(x => x.DocumentDescription, "")
                .Create();

            _classUnderTest.Validate(request).IsValid.Should().BeFalse();
        }

    }
}
