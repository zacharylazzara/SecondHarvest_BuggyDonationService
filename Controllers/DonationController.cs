using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BuggyDonationService.Services;
using BuggyDonationService.Models;

namespace BuggyDonationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonationController : ControllerBase
    {
        private readonly DonationService _donationService;
        private readonly ExternalDataService _externalService;

        public DonationController()
        {
            _donationService = new DonationService();
            _externalService = new ExternalDataService();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateDonation([FromBody] CreateDonationRequest request)
        {
            var result = await _donationService.ProcessDonationAsync(request.DonorName, request.Amount);
            if (result)
            {
                return Ok(new { message = "Donation processed successfully" });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to process donation" });
            }
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDonationStats()
        {
            var externalData = _externalService.GetMarketData().Result;
            var stats = await _donationService.GetStatsAsync();
            
            /* Again we should handle the scenario in which this fails, as it's 
            possible that our external data call fails in which case an Ok status is unacceptable.
            We should verify that we've recieved the external data and then return an Ok status if 
            the data was recieved, otherwise we should return an appropriate error code (such as 404
            if we failed to find the external service for whatever reason, etc). */
            return Ok(new
            {
                stats = stats,
                marketInfo = externalData
            });
        }

        [HttpGet("{donorName}/history")]
        public async Task<IActionResult> GetDonorHistory(string donorName)
        {
            var history = await _donationService.GetDonorHistoryAsync(donorName);

            // We should handle the scenario in which we fail to recieve the history
            return Ok(history);
        }
    }
}