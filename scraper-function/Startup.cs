using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using scraper_function.TableStorage;
using scraper_function.Utils;

[assembly: FunctionsStartup(typeof(scraper_function.Startup))]

namespace scraper_function
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<IScraper, DaftScraper>();
            builder.Services.AddScoped<IMapUtils, GoogleMapsUtils>((s) => new GoogleMapsUtils(Environment.GetEnvironmentVariable("GoogleKey")));
            builder.Services.AddSingleton((s) => new TableStorageClient(Environment.GetEnvironmentVariable("COSMOSDB_CONNECTIONSTRING"), "RentUrls"));
        }
    }
}
