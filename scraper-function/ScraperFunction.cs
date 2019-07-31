using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace scraper_function
{
    public static class ScraperFunction
    {
        [FunctionName("GetLastProperties")]
        public static void Run([TimerTrigger("* */1 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
