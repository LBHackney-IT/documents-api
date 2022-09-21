using System;
using System.Collections.Generic;
using AutoFixture;
using DocumentsApi.V1.Authorization;
using FluentAssertions;
using Hackney.Core.Http;
using Hackney.Core.JWT;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;

namespace DeveloperHubAPI.Tests.V1.Authorization
{
    [TestFixture]
    public class AuthoriseByGroupsTests
    {
        TokenGroupsFilter _classUnderTest;
        Mock<IHttpContextWrapper> _mockContextWrapper;
        Mock<ITokenFactory> _mockTokenFactory;
        private string[] _requiredGoogleGroups;
        private static Fixture _fixture => new Fixture();

        [SetUp]
        public void Init()
        {
            _mockContextWrapper = new Mock<IHttpContextWrapper>();
            _mockTokenFactory = new Mock<ITokenFactory>();
            _requiredGoogleGroups = new string[] { "test_group_name", "some-other-group" };

            Environment.SetEnvironmentVariable("groups", string.Join(",", _requiredGoogleGroups));

            _classUnderTest = new TokenGroupsFilter(_mockContextWrapper.Object, _mockTokenFactory.Object, "groups");
        }

        [Test]
        public void ConstructorThrowsErrorIfGroupsEnvVariableIsNull()
        {
            var incorrectEnvVariable = "var";

            Func<TokenGroupsFilter> func = () => new TokenGroupsFilter(_mockContextWrapper.Object, _mockTokenFactory.Object, incorrectEnvVariable);

            func.Should().Throw<EnvironmentVariableNullException>().WithMessage($"Cannot resolve {incorrectEnvVariable} environment variable.");
        }

        private (AuthorizationFilterContext, HeaderDictionary) SetUpMockContextAndHeaders()
        {
            var mockHttpContext = new Mock<HttpContext>();
            var actionContext = new ActionContext(mockHttpContext.Object,
                                                  new Microsoft.AspNetCore.Routing.RouteData(),
                                                  new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());

            var requestHeaders = new HeaderDictionary(new Dictionary<string, StringValues> { { "Authorization", "abc" } });
            _mockContextWrapper.Setup(x => x.GetContextRequestHeaders(context.HttpContext)).Returns(requestHeaders);

            return (context, requestHeaders);
        }

        [Test]
        public void OnAuthorizationResultIsUnauthorizedIfTokenIsNull()
        {
            // Arrange
            (var context, var requestHeaders) = SetUpMockContextAndHeaders();
            _mockTokenFactory.Setup(x => x.Create(requestHeaders, "Authorization")).Returns((Token) null);
            // Act
            _classUnderTest.OnAuthorization(context);
            // Assert
            context.Result.Should().BeOfType(typeof(UnauthorizedObjectResult));
            (context.Result as UnauthorizedObjectResult).Value.Should().Be("User  is not authorized to access this endpoint.");
            _mockTokenFactory.Verify(x => x.Create(requestHeaders, "Authorization"), Times.Once);
        }

        [Test]
        public void OnAuthorizationResultIsUnauthorizedIfTokenDoesNotContainRequiredGoogleGroups()
        {
            // Arrange
            (var context, var requestHeaders) = SetUpMockContextAndHeaders();
            var userToken = _fixture.Build<Token>().With(x => x.Groups, new string[] { "not one of the allowed groups" }).Create();
            _mockTokenFactory.Setup(x => x.Create(requestHeaders, "Authorization")).Returns((Token) userToken);
            // Act
            _classUnderTest.OnAuthorization(context);
            // Assert
            context.Result.Should().BeOfType(typeof(UnauthorizedObjectResult));
            (context.Result as UnauthorizedObjectResult).Value.Should().Be($"User {userToken.Name} is not authorized to access this endpoint.");
            _mockTokenFactory.Verify(x => x.Create(requestHeaders, "Authorization"), Times.Once);
        }

        [Test]
        public void OnAuthorizationResultIsNullIfTokenContainsOneOfTheRequiredGoogleGroups()
        {
            // Arrange
            (var context, var requestHeaders) = SetUpMockContextAndHeaders();
            var userToken = _fixture.Build<Token>().With(x => x.Groups, new string[] { "test_group_name" }).Create();
            _mockTokenFactory.Setup(x => x.Create(requestHeaders, "Authorization")).Returns((Token) userToken);
            // Act
            _classUnderTest.OnAuthorization(context);
            // Assert
            context.Result.Should().BeNull();
            _mockTokenFactory.Verify(x => x.Create(requestHeaders, "Authorization"), Times.Once);
        }

    }
}
