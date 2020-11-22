using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Playground
{
    public class MetricsCollector
    {
        private static HttpClient httpClient = new HttpClient();

        PerformanceCounter cpuCounter;
        PerformanceCounter ramCounter;

        public MetricsCollector()
        {
            httpClient.DefaultRequestHeaders.Add("ApiKey", "KR6EdgVl46yvV3fDIEjrgdBgzwcYxpLBZHTxQPLfy2g=");

            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        }

        public async Task Start()
        {
            while (true)
            {
                await SendRequest("pc-performance", "Cpu", (long)cpuCounter.NextValue());
                await SendRequest("pc-performance", "Available Ram", (long)ramCounter.NextValue());

                Thread.Sleep(1000);
            }
        }

        private async Task<bool> SendRequest(string @namespace, string metricName, long value)
        {
            string url = $"https://localhost:44344/apimetric/add/{@namespace}/{metricName}/{value}";

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
                    await Task.Delay(10 * 500);
                }
            }

            return response.IsSuccessStatusCode;
        }
    }
}
