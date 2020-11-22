using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Metricaly.Infrastructure.Services;
using Metricaly.Core.Interfaces;

namespace Infrastructure.UnitTests.Services
{
    public class ApiKeyGeneratorTests
    {
        private readonly IApiKeyGenerator apiKeyGenerator;

        public ApiKeyGeneratorTests()
        {
            apiKeyGenerator = new ApiKeyGenerator();
        }

        [Test]
        public void ShouldReturnApiKey()
        {
            var apiKey = apiKeyGenerator.Generate();

            apiKey.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void ShouldReturnUniqueApiKey()
        {
            var apiKey1 = apiKeyGenerator.Generate();
            var apiKey2 = apiKeyGenerator.Generate();

            apiKey1.Should().NotBeEquivalentTo(apiKey2);
        }

        [Test]
        public void ShouldReturn44CharactersInLengthApiKey()
        {
            var apiKey = apiKeyGenerator.Generate();

            apiKey.Should().HaveLength(44);
        }
    }
}
