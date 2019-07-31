namespace scraper_function.Utils
{
    public interface IMapUtils
    {
        int GetDistance((string lat, string lng)? coordinates);
        (string lat, string lng)? GetMapLocation(string location);
        string GetMapUrl((string lat, string lng)? coordinates, string location);
    }
}