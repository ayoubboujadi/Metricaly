using System;
using System.Collections.Generic;

namespace Metricaly.Infrastructure.Common.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; set; }
        public IDictionary<string, string> Errors { get; }

        public ApiException(string message,
                            int statusCode = 500,
                            IDictionary<string, string> errors = null):
            base(message)
        {
            StatusCode = statusCode;
            Errors = errors;
        }

        public ApiException(Exception ex, int statusCode = 500) : base(ex.Message)
        {
            StatusCode = statusCode;
        }
    }
}
