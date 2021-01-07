using AutoFixture;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.Infrastructure;

namespace DocumentsApi.Tests.V1
{
    public static class TestDataHelper
    {
        private static Fixture _fixture = new Fixture();

        public static Document CreateDocument()
        {
            return _fixture.Build<Document>()
                .Without(x => x.Id)
                .Without(x => x.CreatedAt)
                .Create();
        }

        public static Claim CreateClaim()
        {
            var document = CreateDocument();
            return _fixture.Build<Claim>()
                .Without(x => x.Id)
                .Without(x => x.CreatedAt)
                .With(x => x.Document, document)
                .Create();
        }
    }
}
