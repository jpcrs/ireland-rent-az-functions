using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using scraper_function.Model;
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
            site = "https://www.daft.ie/";
        }
        public async Task<IEnumerable<MessageModel>> GetRents()
        {
            var configuration = Configuration.Default.WithDefaultLoader();
            var doc = await BrowsingContext.New(configuration).OpenAsync("https://www.daft.ie/dublin-city/residential-property-for-rent/dublin-1,dublin-2,dublin-4,dublin-6,dublin-6w,dublin-8/?ad_type=rental&advanced=1&s%5Bmxp%5D=1200&s%5Bmnb%5D=1&s%5Bmnbt%5D=1&s%5Bphotos%5D=1&s%5Badvanced%5D=1&s%5Bpt_id%5D%5B0%5D=1&s%5Bpt_id%5D%5B1%5D=2&s%5Bpt_id%5D%5B2%5D=3&s%5Bpt_id%5D%5B3%5D=4&searchSource=rental");
            var rents = doc.QuerySelectorAll(".box").Where(t => tableStorageClient.CheckIfInCache(site + t.QuerySelector(".search_result_title_box a").GetAttribute("href").Replace("'", "")) == false).Take(10);
            var models = new List<MessageModel>();
            foreach (var rent in rents)
            {
                var titulo = FormatTitle(rent);
                var coordinates = mapUtils.GetMapLocation(titulo);
                var distance = coordinates.HasValue ? mapUtils.GetDistance(coordinates) : 1;
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
                    Distance = distance
                };
                tableStorageClient.InsertRent(model);
                models.Add(model);
            }

            return new List<MessageModel>();
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
            var config = Configuration.Default.WithDefaultLoader();
            var doc = await BrowsingContext.New(config).OpenAsync(link);
            var imgs = doc.QuerySelectorAll(".pbxl_carousel_item img");
            return imgs?.Select(img => img.GetAttribute("src")).ToList();
        }
    }
}
