using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using scraper_function.Model;
using scraper_function.TableStorage;
using scraper_function.Utils;

namespace scraper_function
{
    public class DaftScraper : IScraper
    {
        private readonly TableStorageClient tableStorageClient;
        private readonly IMapUtils mapUtils;
        private readonly string site;

        public DaftScraper(TableStorageClient tableStorageClient, IMapUtils mapUtils)
        {
            this.tableStorageClient = tableStorageClient;
            this.mapUtils = mapUtils;
            site = "https://www.daft.ie";
        }
        public async Task<IEnumerable<MessageModel>> GetRents()
        {
            var configuration = AngleSharp.Configuration.Default.WithDefaultLoader();
            var doc = await BrowsingContext.New(configuration).OpenAsync(Environment.GetEnvironmentVariable("DaftUrl"));
            var rents = doc.QuerySelectorAll(".box").Where(t => tableStorageClient.CheckIfInCache(site + t.QuerySelector(".search_result_title_box a").GetAttribute("href").Replace("'", "")) == false).Take(2);

            var models = new ConcurrentBag<MessageModel>();
            var proccessRents = rents.Select(async rent =>
            {
                var titulo = FormatTitle(rent);
                var coordinates = await mapUtils.GetMapLocation(titulo);
                var distance = coordinates.HasValue ? await mapUtils.GetDistance(coordinates) : 1;
                var timeToWork = coordinates.HasValue ? await mapUtils.GetMyWorkDistance(coordinates) : "";
                var link = site + rent.QuerySelector(".search_result_title_box a").GetAttribute("href").Replace("'", "");
                var model = new MessageModel
                {
                    Link = link,
                    Price = rent.QuerySelector(".price").Text(),
                    Location = titulo,
                    Coordinates = coordinates,
                    Map = mapUtils.GetMapUrl(coordinates, titulo),
                    InsertDate = DateTime.Now,
                    Photos = await GetPhotos(link),
                    Distance = distance,
                    WorkDistance = timeToWork
                };
                models.Add(model);
            });
            await Task.WhenAll(proccessRents);

            return models;
        }

        private string FormatTitle(IElement topico)
        {
            var titulo = topico.QuerySelector(".search_result_title_box a").Text().Replace("\t", "")
                .Replace("\n", "").Replace("  ", "").Replace(@"\", "").Replace("'", "");
            titulo = titulo.Substring(0, titulo.IndexOf("-", StringComparison.Ordinal));
            return titulo;
        }

        private async Task<List<string>> GetPhotos(string link)
        {
            var config = AngleSharp.Configuration.Default.WithDefaultLoader();
            var doc = await BrowsingContext.New(config).OpenAsync(link);
            var imgs = doc.QuerySelectorAll(".pbxl_carousel_item img");
            return imgs?.Select(img => img.GetAttribute("src")).ToList();
        }
    }
}
