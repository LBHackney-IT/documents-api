using System;
using System.Linq;
using AutoFixture;
using DocumentsApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace DocumentsApi.Tests.V1.Infrastructure
{
    [TestFixture]
    public class DatabaseContextTest : DatabaseTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Test]
        public void CanGetADatabaseEntity()
        {
            var databaseEntity = _fixture.Build<DocumentEntity>()
                .Without(x => x.Id)
                .Without(x => x.CreatedAt)
                .Create();

            DatabaseContext.Add(databaseEntity);
            DatabaseContext.SaveChanges();

            var result = DatabaseContext.DocumentEntities.ToList().FirstOrDefault();

            result.Should().Be(databaseEntity);
            result?.Id.Should().NotBeEmpty();
            result?.CreatedAt.Should().BeCloseTo(DateTime.Now, 1000);
        }
    }
}
