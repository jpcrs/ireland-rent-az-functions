using System.Threading.Tasks;

namespace scraper_function.Utils
{
    public interface IMapUtils
    {
        Task<int> GetDistance((string lat, string lng)? coordinates);
        Task<(string lat, string lng)?> GetMapLocation(string location);
        string GetMapUrl((string lat, string lng)? coordinates, string location);
        Task<string> GetMyWorkDistance((string lat, string lng)? coordinates);
    }
}