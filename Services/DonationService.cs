using System.Threading.Tasks;
using System.Collections.Generic;
using BuggyDonationService.Repositories;
using BuggyDonationService.Models;

namespace BuggyDonationService.Services
{
    public class DonationService
    {
        private readonly DonationRepository _repository;

        public DonationService()
        {
            _repository = new DonationRepository();
        }

        public async Task<bool> ProcessDonationAsync(string donorName, decimal amount)
        {
            await Task.Delay(50); // This delay seems unnecessary, we should be able to remove this.
            
            var donation = new Donation
            {
                DonorName = donorName,
                Amount = amount,
                CreatedDate = System.DateTime.Now
            };

            return _repository.SaveDonation(donation); // SaveDonation now returns a boolean indicating success or failure.
        }

        public async Task<DonationStats> GetStatsAsync()
        {
            await Task.Delay(100); // This delay seems unnecessary, we should be able to remove this.
            
            return new DonationStats
            {
                TotalCount = _repository.GetDonationCount(),
                TotalAmount = _repository.GetTotalAmount()
            };
        }

        public async Task<List<Donation>> GetDonorHistoryAsync(string donorName)
        {
            await Task.Delay(25); // This delay seems unnecessary, we should be able to remove this.
            return _repository.GetDonationsByDonor(donorName);
        }
    }
}