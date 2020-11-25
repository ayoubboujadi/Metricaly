using Metricaly.Core.Entities;
using Metricaly.Infrastructure.Common.Mappings;
using System;

namespace Metricaly.Infrastructure.Dtos
{
    public class ApplicationDto : IMapFrom<Application>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ObfuscatedApiKey
        {
            get => string.IsNullOrWhiteSpace(ApiKey) ? string.Empty : ApiKey.Substring(0, 4) + "***************************" + ApiKey.Substring(ApiKey.Length - 4, 4);
        }
    }
}
