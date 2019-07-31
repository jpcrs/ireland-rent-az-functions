using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using scraper_function.Utils;

namespace scraper_function
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<IScraper, DaftScraper>();
            builder.Services.AddScoped<IMapUtils, GoogleMapsUtils>();
            builder.Services.AddSingleton((s) => new TableStorageClient());
        }
    }
}
