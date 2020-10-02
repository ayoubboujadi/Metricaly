using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Metricaly.Core.Interfaces
{
    public interface IApplicationUser
    {
        string Id { get; set; }
       // string Name { get; set; }
        string Email { get; set; }
    }

    public interface ITokenClaimsService
    {
        Task<string> GetTokenAsync(IApplicationUser user, string tokenSecret);
    }
}
