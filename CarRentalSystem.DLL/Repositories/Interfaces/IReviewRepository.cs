using CarRentalSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<Review> AddAsync(Review review);
        Task<bool> DeleteAsync(int id);
        Task<Review> GetByIdAsync(int id);
        Task<List<Review>> GetCarReviewsAsync(int carId);
        Task<List<Review>> GetUserReviewsAsync(string userId);
        Task<Review> GetByBookingIdAsync(int bookingId);
        Task<List<Review>> GetAllReviewsAsync();
        Task<double> GetAverageRatingAsync(int carId);
    }
}
