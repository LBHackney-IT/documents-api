using System;
using System.Linq;
using System.Collections.Generic;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.Gateways;
using DocumentsApi.V1.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace DocumentsApi.Tests.V1.Gateways
{
    [TestFixture]
    public class DocumentsGatewayTests : DatabaseTests
    {
        private DocumentsGateway _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _classUnderTest = new DocumentsGateway(DatabaseContext);
        }

        [Test]
        public void CreatingADocumentShouldInsertIntoTheDatabase()
        {
            var request = TestDataHelper.CreateDocument();
            var query = DatabaseContext.Documents;

            _classUnderTest.SaveDocument(request);

            query.Count()
                .Should()
                .Be(1);

            var foundRecord = query.First();
            foundRecord.Id.Should().NotBeEmpty();
            foundRecord.FileSize.Should().Be(request.FileSize);
            foundRecord.FileType.Should().Be(request.FileType);
        }

        [Test]
        public void CreatingADocumentShouldReturnCreatedDocument()
        {
            var request = TestDataHelper.CreateDocument();

            var created = _classUnderTest.SaveDocument(request);

            created.FileSize.Should().Be(request.FileSize);
            created.FileType.Should().Be(request.FileType);
            created.Id.Should().NotBeEmpty();
            created.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, 1000);
        }

        [Test]
        public void CreatingAClaimShouldInsertIntoTheDatabase()
        {
            var request = TestDataHelper.CreateClaim();
            var query = DatabaseContext.Claims;

            _classUnderTest.CreateClaim(request);

            query.Count()
                .Should()
                .Be(1);

            var foundRecord = query.First();
            foundRecord.Id.Should().NotBeEmpty();
        }

        [Test]
        public void CreatingAClaimShouldReturnCreatedDocument()
        {
            var request = TestDataHelper.CreateClaim();

            var created = _classUnderTest.CreateClaim(request);

            created.Id.Should().NotBeEmpty();
            created.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, 1000);
        }

        [Test]
        public void CanFindADocument()
        {
            var document = TestDataHelper.CreateDocument().ToEntity();
            DatabaseContext.Add(document);
            DatabaseContext.SaveChanges();

            var found = _classUnderTest.FindDocument(document.Id);

            found.Should().BeEquivalentTo(document, opt => opt.ExcludingMissingMembers());
        }

        [Test]
        public void FindingADocumentReturnsNullWhenNotFound()
        {
            var found = _classUnderTest.FindDocument(Guid.NewGuid());

            found.Should().BeNull();
        }

        [Test]
        public void CanFindAClaim()
        {
            var claim = TestDataHelper.CreateClaim().ToEntity();
            var document = TestDataHelper.CreateDocument().ToEntity();
            DatabaseContext.Add(document);
            DatabaseContext.Add(claim);
            DatabaseContext.SaveChanges();

            var found = _classUnderTest.FindClaim(claim.Id);

            found.Should().BeEquivalentTo(claim, opt => opt.ExcludingMissingMembers());
            found.Document.Should().BeEquivalentTo(claim.Document.ToDomain());
        }

        [Test]
        public void FindingAClaimReturnsNullWhenNotFound()
        {
            var found = _classUnderTest.FindClaim(Guid.NewGuid());

            found.Should().BeNull();
        }

        [Test]
        public void CanFindPaginatedClaimsByGroupIdIfCursorIsNull()
        {
            var groupId = new Guid("b81a61ed-f74f-4598-8d82-8155c867a74c");
            var claimEntity1 = TestDataHelper.CreateClaim().ToEntity();
            var claimEntity2 = TestDataHelper.CreateClaim().ToEntity();
            claimEntity1.Id = new Guid("381a6cbf-fef4-403d-85a5-ab48b2a2ccb2");
            claimEntity2.Id = new Guid("51907ce4-2d4a-4466-a5d7-f2668623b49f");
            claimEntity1.CreatedAt = DateTime.UtcNow;
            claimEntity2.CreatedAt = claimEntity1.CreatedAt; //same date to demonstrate that it's picked up because of different ids
            claimEntity1.GroupId = groupId;
            claimEntity2.GroupId = groupId;
            DatabaseContext.Add(claimEntity1);
            DatabaseContext.Add(claimEntity2);
            DatabaseContext.SaveChanges();

            var expected = new List<Claim>()
            {
                claimEntity1.ToDomain(),
                claimEntity2.ToDomain()
            };

            var found = _classUnderTest.FindPaginatedClaimsByGroupId(groupId, 10, null, null);

            found.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        [Test]
        public void CanFindNextPaginatedClaimsByGroupId()
        {
            var groupId = new Guid("b81a61ed-f74f-4598-8d82-8155c867a74c");
            var claimEntity1 = TestDataHelper.CreateClaim().ToEntity();
            var claimEntity2 = TestDataHelper.CreateClaim().ToEntity();
            var claimEntity3 = TestDataHelper.CreateClaim().ToEntity();
            claimEntity1.GroupId = groupId;
            claimEntity2.GroupId = groupId;
            claimEntity3.GroupId = groupId;
            claimEntity1.Id = new Guid("381a6cbf-fef4-403d-85a5-ab48b2a2ccb2");
            claimEntity2.Id = new Guid("51907ce4-2d4a-4466-a5d7-f2668623b49f");
            claimEntity1.CreatedAt = DateTime.UtcNow;
            claimEntity2.CreatedAt = claimEntity1.CreatedAt;
            claimEntity3.CreatedAt = DateTime.UtcNow.AddDays(2);
            DatabaseContext.Add(claimEntity1);
            DatabaseContext.Add(claimEntity2);
            DatabaseContext.Add(claimEntity3);
            DatabaseContext.SaveChanges();

            var expected = new List<Claim>()
            {
                claimEntity1.ToDomain(),
                claimEntity2.ToDomain()
            };

            var found = _classUnderTest.FindPaginatedClaimsByGroupId(groupId, 3, claimEntity3.Id, true);

            found.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        [Test]
        public void CanFindPreviousPaginatedClaimsByGroupId()
        {
            var groupId = new Guid("b81a61ed-f74f-4598-8d82-8155c867a74c");
            var claimEntity1 = TestDataHelper.CreateClaim().ToEntity();
            var claimEntity2 = TestDataHelper.CreateClaim().ToEntity();
            var claimEntity3 = TestDataHelper.CreateClaim().ToEntity();
            claimEntity1.GroupId = groupId;
            claimEntity2.GroupId = groupId;
            claimEntity3.GroupId = groupId;
            claimEntity1.Id = new Guid("381a6cbf-fef4-403d-85a5-ab48b2a2ccb2");
            claimEntity2.Id = new Guid("51907ce4-2d4a-4466-a5d7-f2668623b49f");
            claimEntity1.CreatedAt = DateTime.UtcNow;
            claimEntity2.CreatedAt = claimEntity1.CreatedAt;
            claimEntity3.CreatedAt = DateTime.UtcNow.AddDays(-2);
            DatabaseContext.Add(claimEntity1);
            DatabaseContext.Add(claimEntity2);
            DatabaseContext.Add(claimEntity3);
            DatabaseContext.SaveChanges();

            var expected = new List<Claim>()
            {
                claimEntity1.ToDomain(),
                claimEntity2.ToDomain()

            };

            var found = _classUnderTest.FindPaginatedClaimsByGroupId(groupId, 3, claimEntity3.Id, false);

            found.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        [Test]
        public void CanFindNextPageWhenRecordsHaveSameDateAsCursor()
        {
            var groupId = new Guid("b81a61ed-f74f-4598-8d82-8155c867a74c");
            var claimEntity1 = TestDataHelper.CreateClaim().ToEntity();
            var claimEntity2 = TestDataHelper.CreateClaim().ToEntity();
            var claimEntity3 = TestDataHelper.CreateClaim().ToEntity();
            claimEntity1.GroupId = groupId;
            claimEntity2.GroupId = groupId;
            claimEntity3.GroupId = groupId;
            claimEntity1.Id = new Guid("381a6cbf-fef4-403d-85a5-ab48b2a2ccb2");
            claimEntity2.Id = new Guid("51907ce4-2d4a-4466-a5d7-f2668623b49f");
            claimEntity3.Id = new Guid("781a6cbf-fef4-403d-85a5-ab48b2a2ccb2");
            claimEntity1.CreatedAt = DateTime.UtcNow;
            claimEntity2.CreatedAt = claimEntity1.CreatedAt;
            claimEntity3.CreatedAt = claimEntity1.CreatedAt;
            DatabaseContext.Add(claimEntity1);
            DatabaseContext.Add(claimEntity2);
            DatabaseContext.Add(claimEntity3);
            DatabaseContext.SaveChanges();

            var expected = new List<Claim>()
            {
                claimEntity1.ToDomain(),
                claimEntity2.ToDomain()

            };

            var found = _classUnderTest.FindPaginatedClaimsByGroupId(groupId, 3, claimEntity3.Id, false);

            found.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        [Test]
        public void CanFindPreviousPageWhenRecordsHaveSameDateAsCursor()
        {
            var groupId = new Guid("b81a61ed-f74f-4598-8d82-8155c867a74c");
            var claimEntity1 = TestDataHelper.CreateClaim().ToEntity();
            var claimEntity2 = TestDataHelper.CreateClaim().ToEntity();
            var claimEntity3 = TestDataHelper.CreateClaim().ToEntity();
            claimEntity1.GroupId = groupId;
            claimEntity2.GroupId = groupId;
            claimEntity3.GroupId = groupId;
            claimEntity1.Id = new Guid("381a6cbf-fef4-403d-85a5-ab48b2a2ccb2");
            claimEntity2.Id = new Guid("51907ce4-2d4a-4466-a5d7-f2668623b49f");
            claimEntity3.Id = new Guid("71907ce4-2d4a-4466-a5d7-f2668623b49f");
            claimEntity1.CreatedAt = new DateTime(2022, 09, 01);
            claimEntity2.CreatedAt = claimEntity1.CreatedAt;
            claimEntity3.CreatedAt = claimEntity1.CreatedAt;
            DatabaseContext.Add(claimEntity1);
            DatabaseContext.Add(claimEntity2);
            DatabaseContext.Add(claimEntity3);
            DatabaseContext.SaveChanges();

            var expected = new List<Claim>()
            {
                claimEntity1.ToDomain(),
                claimEntity2.ToDomain()
            };

            var found = _classUnderTest.FindPaginatedClaimsByGroupId(groupId, 3, claimEntity3.Id, false);

            found.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        [Test]
        public void ReturnsAllClaimsIfNoLimitSpecified()
        {
            var groupId = new Guid("b81a61ed-f74f-4598-8d82-8155c867a74c");
            var claimEntity1 = TestDataHelper.CreateClaim().ToEntity();
            var claimEntity2 = TestDataHelper.CreateClaim().ToEntity();
            var claimEntity3 = TestDataHelper.CreateClaim().ToEntity();
            claimEntity1.GroupId = groupId;
            claimEntity2.GroupId = groupId;
            claimEntity3.GroupId = groupId;
            claimEntity1.CreatedAt = new DateTime(2022, 09, 01);
            claimEntity2.CreatedAt = claimEntity1.CreatedAt.AddHours(1);
            claimEntity3.CreatedAt = claimEntity1.CreatedAt.AddHours(2);
            DatabaseContext.Add(claimEntity1);
            DatabaseContext.Add(claimEntity2);
            DatabaseContext.Add(claimEntity3);
            DatabaseContext.SaveChanges();

            var expected = new List<Claim>()
            {
                claimEntity3.ToDomain(),
                claimEntity2.ToDomain(),
                claimEntity1.ToDomain()
            };

            var found = _classUnderTest.FindPaginatedClaimsByGroupId(groupId, null, null, false);

            found.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        [Test]
        public void ReturnsEmptyCollectionWhenNoClaimContainsSpecifiedGroupId()
        {
            var found = _classUnderTest.FindPaginatedClaimsByGroupId(Guid.NewGuid(), 10, null, null);
            found.Should().BeEmpty();
        }

        [Test]
        public void ReturnsEmptyClaimsCollectionIfGroupIdDoesNotMatch()
        {
            var groupId = new Guid("aff8e4e8-6628-4654-a0e9-140c4b5a5da6");
            var claimEntity1 = TestDataHelper.CreateClaim().ToEntity();
            claimEntity1.GroupId = new Guid("591f0c9e-100c-402b-9344-4c623abc57bb");
            DatabaseContext.Add(claimEntity1);
            DatabaseContext.SaveChanges();

            var found = _classUnderTest.FindPaginatedClaimsByGroupId(groupId, 10, null, null);

            found.Should().BeEmpty();
        }

        [Test]
        public void CanUpdateCLaimsGroupId()
        {
            var oldGroupId = Guid.NewGuid();
            var newGroupId = Guid.NewGuid();
            var claim1 = TestDataHelper.CreateClaim().ToEntity();
            var claim2 = TestDataHelper.CreateClaim().ToEntity();
            var claim3 = TestDataHelper.CreateClaim().ToEntity();
            claim1.GroupId = oldGroupId;
            claim2.GroupId = oldGroupId;
            claim3.GroupId = oldGroupId;
            DatabaseContext.Add(claim1);
            DatabaseContext.Add(claim2);
            DatabaseContext.Add(claim3);
            DatabaseContext.SaveChanges();

            var result = _classUnderTest.UpdateClaimsGroupId(oldGroupId, newGroupId);

            result[0].GroupId.Should().Be(newGroupId);
            result[1].GroupId.Should().Be(newGroupId);
            result[2].GroupId.Should().Be(newGroupId);
        }

        [Test]
        public void ReturnsEmptyIfNoClaimContainsTheGroupId()
        {
            var oldGroupId = Guid.NewGuid();
            var newGroupId = Guid.NewGuid();
            var randomGroupId = Guid.NewGuid();
            var claim1 = TestDataHelper.CreateClaim().ToEntity();
            var claim2 = TestDataHelper.CreateClaim().ToEntity();
            var claim3 = TestDataHelper.CreateClaim().ToEntity();
            claim1.GroupId = randomGroupId;
            claim2.GroupId = randomGroupId;
            claim3.GroupId = randomGroupId;
            DatabaseContext.Add(claim1);
            DatabaseContext.Add(claim2);
            DatabaseContext.Add(claim3);
            DatabaseContext.SaveChanges();

            var result = _classUnderTest.UpdateClaimsGroupId(oldGroupId, newGroupId);

            result.Count.Should().Be(0);
        }
    }
}
