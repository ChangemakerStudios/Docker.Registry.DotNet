using Docker.Registry.DotNet.Authentication;
using Xunit;

namespace Docker.Registry.DotNet.Tests
{
    public class AuthenticateParserTests
    {
        [Theory]
        [InlineData("realm=test realm,service=test service,scope=test scope", "test realm", "test service", "test scope")]
        [InlineData("realm=\"test realm\",service=\"test service\",scope=\"test scope\"", "test realm", "test service", "test scope")]
        [InlineData("realm=test realm,service=test service,scope=\"scope1,scope2\"", "test realm", "test service", "scope1,scope2")]
        [InlineData("realm=\"test realm\",service=\"test service\",scope=\"scope1,scope2\"", "test realm", "test service", "scope1,scope2")]
        public void GivenACommaDelimitedChallengeHeader_WhenIParseItAsTyped_ThenItShouldReturnTheCorrectSegments(
            string header, string expectedRealm, string expectedService, string expectedScope)
        {
            var actual = AuthenticateParser.ParseTyped(header);
            Assert.Equal(expectedRealm, actual.Realm);
            Assert.Equal(expectedService, actual.Service);
            Assert.Equal(expectedScope, actual.Scope);
        }
    }
}