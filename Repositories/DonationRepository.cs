using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BuggyDonationService.Models;

namespace BuggyDonationService.Repositories
{
    public class DonationRepository
    {
        private readonly string _connectionString;

        public DonationRepository()
        {
            /* This password is insecure (too simple; ideally it should be randomly generated) and
            should not be in plain text like this (anyone with access to the repo can see this). */
            _connectionString = "Server=sql-server-001;Database=CharityDonations;User Id=charity_app;Password=Ch@rity2024!;";
        }

        public bool SaveDonation(Donation donation)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            // We're using a transaction to ensure the donation is properly saved, or rolled back in case of an error.
            using var transaction = connection.BeginTransaction("SaveDonationTransaction");
            try
            {
                using (var command = new SqlCommand("INSERT INTO Donations (DonorName, Amount, CreatedDate) VALUES (@DonorName, @Amount, @CreatedDate)", connection, transaction))
                {
                    // Using parameters to prevent SQL injection attacks.
                    command.Parameters.AddWithValue("@DonorName", donation.DonorName);
                    command.Parameters.AddWithValue("@Amount", donation.Amount);
                    command.Parameters.AddWithValue("@CreatedDate", donation.CreatedDate);

                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
            catch
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex)
                {
                    // TODO: Log the rollback failure (not implemented here, throwing an exception instead for simplicity)
                    throw new Exception("Failed to rollback transaction", ex);
                }
                return false;
            }
            return true;
        }

        public int GetDonationCount()
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            
            var command = new SqlCommand("SELECT COUNT(*) FROM Donations", connection);
            return (int)command.ExecuteScalar();
        }

        public decimal GetTotalAmount()
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            
            var command = new SqlCommand("SELECT ISNULL(SUM(Amount), 0) FROM Donations", connection);
            return (decimal)command.ExecuteScalar();
        }

        public List<Donation> GetDonationsByDonor(string donorName)
        {
            var donations = new List<Donation>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            
            // This query is vulnerable to SQL injection. Use parameterized queries instead.
            var query = $"SELECT Id, DonorName, Amount, CreatedDate FROM Donations WHERE DonorName = '{donorName}'";
            using var command = new SqlCommand(query, connection);
            using var reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                donations.Add(new Donation
                {
                    Id = reader.GetInt32("Id"),
                    DonorName = reader.GetString("DonorName"),
                    Amount = reader.GetDecimal("Amount"),
                    CreatedDate = reader.GetDateTime("CreatedDate")
                });
            }
            
            return donations;
        }
    }
}