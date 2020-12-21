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
        private static Random random = new Random();

        PerformanceCounter cpuCounter;
        PerformanceCounter ramCounter;

        public MetricsCollector()
        {
            httpClient.DefaultRequestHeaders.Add("ApiKey", "+qmG6HhdI7Egmr2kkybxrVojuJjRxsOgVYcpPm58jpU=");

            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        }

        public async Task Start()
        {
            while (true)
            {
                await SendRequest("performance", "Cpu", (long)cpuCounter.NextValue());
                await SendRequest("performance", "AvailableRam", (long)ramCounter.NextValue());

                var count = random.Next(60, 120);
                var latency = random.Next(200, 1100);

                await SendRequest("requests", "Count", count);
                await SendRequest("requests", "Latency", latency);
                await SendRequest("requests", "Bandwidth", count * random.Next(600, 2100));

                await SendRequest("users", "SignUps", random.Next(0, 5) == 0 ? random.Next(0, 5) : 0);

                var inQueueCount = random.Next(30, 51);
                await SendRequest("queue", "InQueueCount", inQueueCount);

                await SendRequest("something", "Level1", random.Next(120, 131));
                await SendRequest("something", "Level2", random.Next(20, 51));
                await SendRequest("something", "Level3", random.Next(30, 41));

                await SendRequest("errors", "ModuleX", random.Next(0, 3) == 0 ? random.Next(0, 5) : 0);
                await SendRequest("errors", "ModuleY", random.Next(0, 4) == 0 ? random.Next(2, 6) : 0);
                await SendRequest("errors", "ModuleZ", random.Next(0, 5) == 0 ? random.Next(0, 10) : 0);

                Thread.Sleep(300);
            }
        }

        private async Task<bool> SendRequest(string @namespace, string metricName, long value)
        {
            string url = $"https://localhost:5001/collect/single/{@namespace}/{metricName}/{value}";

            HttpResponseMessage response;
            while (true)
            {
                try
                {
                    Console.WriteLine($"{DateTime.Now.ToShortTimeString()} sent");
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
