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
        private DateTime _validRetentionDate;

        [SetUp]
        public void SetUp()
        {
            _classUnderTest = new ClaimRequestValidator();
            _validRetentionDate = DateTime.UtcNow.AddDays(1);
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
            var tooLongName = new string('A', 301);
            var request = _fixture.Build<ClaimRequest>()
                .With(x => x.DocumentName, tooLongName)
                .With(x => x.RetentionExpiresAt, _validRetentionDate)
                .Create();

            _classUnderTest.Validate(request).IsValid.Should().BeFalse();
        }

        [Test]
        public void FailsWithDocumentNameShorterThan1Character()
        {
            var request = _fixture.Build<ClaimRequest>()
                .With(x => x.DocumentName, "")
                .With(x => x.RetentionExpiresAt, _validRetentionDate)
                .Create();

            _classUnderTest.Validate(request).IsValid.Should().BeFalse();
        }

        [Test]
        public void FailsWhenDocumentDescriptionLongerThan1000Characters()
        {
            var tooLongDescription = new string('T', 1001);
            var request = _fixture.Build<ClaimRequest>()
                .With(x => x.DocumentDescription, tooLongDescription)
                .With(x => x.RetentionExpiresAt, _validRetentionDate)
                .Create();

            _classUnderTest.Validate(request).IsValid.Should().BeFalse();
        }

        [Test]
        public void AcceptsWhenDocumentDescriptionIsExactly1000Characters()
        {
            var validDescription = new string('T', 1000);
            var requestWithValidDescription = _fixture.Build<ClaimRequest>()
                .With(x => x.DocumentDescription, validDescription)
                .With(x => x.RetentionExpiresAt, _validRetentionDate)
                .Create();

            _classUnderTest.Validate(requestWithValidDescription).IsValid.Should().BeTrue();
        }

        [Test]
        public void FailsWhenDocumentDescriptionShorterThan1Character()
        {
            var request = _fixture.Build<ClaimRequest>()
                .With(x => x.DocumentDescription, "")
                .Create();

            _classUnderTest.Validate(request).IsValid.Should().BeFalse();
        }


        [Test]
        [TestCase(null)]
        public void AcceptsWhenTargetTypeIsNull(string value)
        {
            var request = _fixture.Build<ClaimRequest>()
                .With(x => x.TargetType, value)
                .Create();

            _classUnderTest.Validate(request).IsValid.Should().BeTrue();
        }
        [Test]
        public void FailsWhenTargetTypeIsOver50Character()
        {
            var targetTypeOver50Characters = new string('T', 51);
            var request = _fixture.Build<ClaimRequest>()
                .With(x => x.TargetType, targetTypeOver50Characters)
                .Create();

            _classUnderTest.Validate(request).IsValid.Should().BeFalse();
        }

        [Test]
        public void ValidatesAValidRequest()
        {
            var request = _fixture.Build<ClaimRequest>()
                .With(x => x.RetentionExpiresAt, _validRetentionDate)
                .Create();

            _classUnderTest.Validate(request).IsValid.Should().BeTrue();
        }
    }
}
