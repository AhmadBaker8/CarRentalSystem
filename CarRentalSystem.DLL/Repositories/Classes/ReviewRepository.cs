using CarRentalSystem.DAL.Data;
using CarRentalSystem.DAL.Models;
using CarRentalSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.DAL.Repositories.Classes
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;
        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Review> AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Car)
                .Include(r => r.Booking)
                .FirstOrDefaultAsync(r => r.Id == review.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return false;

            review.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Review> GetByIdAsync(int id)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Car)
                .Include(r => r.Booking)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        }

        public async Task<List<Review>> GetCarReviewsAsync(int carId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.CarId == carId && !r.IsDeleted)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Review>> GetUserReviewsAsync(string userId)
        {
            return await _context.Reviews
                .Include(r => r.Car)
                .Include(r => r.User)
                .Where(r => r.UserId == userId && !r.IsDeleted)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Review> GetByBookingIdAsync(int bookingId)
        {
            return await _context.Reviews
               .Include(r => r.User)
               .Include(r => r.Car)
               .FirstOrDefaultAsync(r => r.BookingId == bookingId && !r.IsDeleted);
        }

        public async Task<List<Review>> GetAllReviewsAsync()
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Car)
                .Where(r => !r.IsDeleted)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingAsync(int carId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.CarId == carId && !r.IsDeleted)
                .ToListAsync();

            if (reviews.Count == 0) return 0;

            return reviews.Average(r => r.Rating);
        }

    }
}
