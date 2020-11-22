using Metricaly.Core.Interfaces;
using System;
using System.Security.Cryptography;

namespace Metricaly.Infrastructure.Services
{
    public class ApiKeyGenerator : IApiKeyGenerator
    {
        public string Generate()
        {
            var key = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(key);
            var result = Convert.ToBase64String(key);

            return result;
        }
    }
}
