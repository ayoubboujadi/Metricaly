using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Metricaly.Infrastructure.Common.Models
{
    public class ApiProblemDetails : ProblemDetails
    {
        public IDictionary<string, string> Errors { get; set; }
    }
}
