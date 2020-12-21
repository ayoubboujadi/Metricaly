using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Playground
{
    public class MetricsCollectorExample
    {
        private static HttpClient httpClient = new HttpClient();
        private static Random random = new Random();

        public MetricsCollectorExample()
        {
            httpClient.DefaultRequestHeaders.Add("ApiKey", "KR6EdgVl46yvV3fDIEjrgdBgzwcYxpLBZHTxQPLfy2g=");
        }

        public async Task Start()
        {
            while (true)
            {
                var count = random.Next(3, 10);
                var latency = random.Next(500, 1500);

                await SendRequest("requests", "count", count);
                await SendRequest("requests", "latency", latency);
                await SendRequest("requests", "bandwidth", count * random.Next(600, 2100));


                await SendRequest("requests", "count", count);
                await SendRequest("requests", "latency", latency);
                await SendRequest("requests", "bandwidth", count * random.Next(600, 2100));

                Thread.Sleep(500);
            }
        }

        private async Task<bool> SendRequest(string @namespace, string metricName, long value)
        {
            string url = $"https://localhost:44334/collect/single/{@namespace}/{metricName}/{value}";

            HttpResponseMessage response;
            while (true)
            {
                try
                {
                    response = await httpClient.GetAsync(url);
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now} Error, trying again in 10 seconds.");
                    await Task.Delay(10 * 1000);
                }
            }

            return response.IsSuccessStatusCode;
        }
    }
}
