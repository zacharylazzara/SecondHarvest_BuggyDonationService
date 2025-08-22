using System;
using System.ComponentModel.DataAnnotations;

namespace BuggyDonationService.Models
{
    public class Donation
    {
        public int Id { get; set; }
        
        [Required]
        public string DonorName { get; set; }
        
        [Range(0.01, 10000)]
        public decimal Amount { get; set; }
        
        public DateTime CreatedDate { get; set; }
    }

    public class CreateDonationRequest
    {
        [Required]
        public string DonorName { get; set; }
        
        [Required]
        [Range(0.01, 10000)]
        public decimal Amount { get; set; }
    }

    public class DonationStats
    {
        public int TotalCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}