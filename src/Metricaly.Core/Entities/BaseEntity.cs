using System;
using System.Collections.Generic;
using System.Text;

namespace Metricaly.Core.Entities
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            CreatedDate = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
