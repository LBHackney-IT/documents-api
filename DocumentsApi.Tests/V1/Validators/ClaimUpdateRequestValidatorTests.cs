using FluentAssertions;
using NUnit.Framework;
using AutoFixture;
using DocumentsApi.V1.Validators;
using DocumentsApi.V1.Boundary.Request;
using System;

namespace DocumentsApi.Tests.V1.Validators
{
    [TestFixture]
    public class ClaimUpdateRequestValidatorTests
    {
        private Fixture _fixture = new Fixture();
        private ClaimUpdateRequestValidator _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _classUnderTest = new ClaimUpdateRequestValidator();
        }

        [Test]
        public void FailsWithNoParameters()
        {
            var request = _fixture.Build<ClaimUpdateRequest>()
                .With(x => x.GroupId, () => null)
                .With(x => x.ValidUntil, () => null)
                .Create();

            _classUnderTest.Validate(request).IsValid.Should().BeFalse();
        }
    }
}
