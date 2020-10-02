using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
