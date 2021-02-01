using System;
using DocumentsApi.V1.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace DocumentsApi.Tests.V1.Domain
{
    [TestFixture]
    public class ClaimTests
    {
        [Test]
        public void ExpiresIsTrueWhenExpired()
        {
            var claim = new Claim { RetentionExpiresAt = DateTime.Now.AddDays(-2) };

            claim.Expired.Should().BeTrue();
        }

        [Test]
        public void ExpiresIsFalseWhenNotExpired()
        {
            var claim = new Claim { RetentionExpiresAt = DateTime.Now.AddDays(2) };

            claim.Expired.Should().BeFalse();
        }

    }
}
