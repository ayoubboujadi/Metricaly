using Metricaly.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Metricaly.Core.Interfaces
{
    public interface ICreateApplicationService
    {
        Task CreateAsync(string applicationName, string userId);
    }
}
