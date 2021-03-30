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
        public void ValidatesAValidRequest()
        {
            var request = _fixture.Build<ClaimRequest>()
                .With(x => x.RetentionExpiresAt, DateTime.UtcNow.AddDays(1))
                .Create();

            _classUnderTest.Validate(request).IsValid.Should().BeTrue();
        }
    }
}
