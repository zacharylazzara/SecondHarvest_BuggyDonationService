using System.Net.Http;
using System.Threading.Tasks;

namespace BuggyDonationService.Services
{
    public class ExternalDataService
    {
        public async Task<string> GetMarketData()
        {
            var client = new HttpClient();
            var response = await client.GetStringAsync("https://api.marketdata.org/charity-insights");
            return response;
        }

        public async Task<string> GetDonationTrends()
        {
            var client = new HttpClient();
            return await client.GetStringAsync("https://api.givingtrends.org/monthly-report");
        }
    }
}