using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace scraper_function.Utils
{
    public class GoogleMapsUtils : IMapUtils
    {
        private readonly string apiKey;
        public GoogleMapsUtils(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public (string lat, string lng)? GetMapLocation(string location)
        {
            var loc = location.Replace(" ", "+");
            var coordinateUrl = $"https://maps.google.com/maps/api/geocode/json?address={loc}&key={apiKey}";
            var coordinates = GetCoordinates(coordinateUrl);
            return coordinates;
        }

        public string GetMapUrl((string lat, string lng)? coordinates, string location)
        {
            var loc = location.Replace(" ", "+");
            return coordinates != null
                ? $"https://maps.googleapis.com/maps/api/staticmap?center={loc}&zoom=12&size=800x400&markers=color:blue%7Clabel:S%7C{coordinates.Value.lat},{coordinates.Value.lng}"
                : null;
        }

        private (string lat, string lng)? GetCoordinates(string url)
        {
            try
            {
                var response = new System.Net.WebClient().DownloadString(url);
                var root = JsonConvert.DeserializeObject<MapCoordinates>(response);
                var result = root.results.FirstOrDefault();

                if (result != null)
                    return (result.geometry.location.lat.ToString(CultureInfo.InvariantCulture),
                        result.geometry.location.lng.ToString(CultureInfo.InvariantCulture));

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public int GetDistance((string lat, string lng)? coordinates)
        {
            string latCenter = "53.349722";
            string lngCenter = "-6.260278";
            var url =
                $"https://maps.googleapis.com/maps/api/distancematrix/json?units=metric&origins={coordinates.Value.lat},{coordinates.Value.lng}" +
                $"&destinations={latCenter},{lngCenter}&transit_routing_preference=less_walking&mode=walking&key=key={apiKey}";

            try
            {
                var response = new System.Net.WebClient().DownloadString(url);
                var root = JsonConvert.DeserializeObject<MapDistance>(response);
                var result = root.rows.FirstOrDefault();

                var element = result?.elements.FirstOrDefault();
                if (element != null)
                {
                    return element.distance.value;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0;
            }

            return 0;
        }
    }

    public class Distance
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class Duration
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class Element
    {
        public Distance distance { get; set; }
        public Duration duration { get; set; }
        public string status { get; set; }
    }

    public class Row
    {
        public List<Element> elements { get; set; }
    }

    public class MapDistance
    {
        public List<string> destination_addresses { get; set; }
        public List<string> origin_addresses { get; set; }
        public List<Row> rows { get; set; }
        public string status { get; set; }
    }

    public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }

    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Northeast
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Southwest
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Viewport
    {
        public Northeast northeast { get; set; }
        public Southwest southwest { get; set; }
    }

    public class Geometry
    {
        public Location location { get; set; }
        public string location_type { get; set; }
        public Viewport viewport { get; set; }
    }

    public class Result
    {
        public List<AddressComponent> address_components { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public string place_id { get; set; }
        public List<string> types { get; set; }
    }

    public class MapCoordinates
    {
        public List<Result> results { get; set; }
        public string status { get; set; }
    }
}
