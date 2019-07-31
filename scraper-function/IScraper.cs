using scraper_function.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace scraper_function
{
    public interface IScraper
    {
        Task<IEnumerable<MessageModel>> GetRents();
    }
}