using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using scraper_function.TableStorage;

namespace scraper_function
{
    public class Function
    {
        private readonly IScraper scraper;
        private EventGridClient client;
        private string topicHostname;

        public Function(IScraper scraper)
        {
            this.scraper = scraper;
            InitEventGrid();
        }

        private void InitEventGrid()
        {
            topicHostname = new Uri(Environment.GetEnvironmentVariable("EventGridEndpoint")).Host;
            TopicCredentials topicCredentials = new TopicCredentials(Environment.GetEnvironmentVariable("EventGridKey"));
            client = new EventGridClient(topicCredentials);
        }

        [FunctionName("GetLastProperties")]
        public async Task Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            var rents = await scraper.GetRents();
            foreach (var rent in rents)
            {
                var x = JsonConvert.SerializeObject(rent);
                await client.PublishEventsAsync(topicHostname, new List<EventGridEvent>()
                    {
                        new EventGridEvent()
                        {
                            Id = Guid.NewGuid().ToString(),
                            EventType = "DublinRent.Rent.NewRent",
                            Data = rent,
                            EventTime = DateTime.Now,
                            Subject = "NewRent",
                            DataVersion = "1"
                        }
                    }
                );
            }
        }
    }
}
