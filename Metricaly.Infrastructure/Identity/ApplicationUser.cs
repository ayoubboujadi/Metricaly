using Metricaly.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Metricaly.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser, IApplicationUser
    {
       // public string Name { get; set; }
    }
}
