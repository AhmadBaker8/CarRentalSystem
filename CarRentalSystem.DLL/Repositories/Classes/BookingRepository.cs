using CarRentalSystem.DAL.Data;
using CarRentalSystem.DAL.Models;
using CarRentalSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.Repositories.Classes
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;
        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<Booking> AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<int> UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return false;

            booking.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Booking> GetByIdAsync(int id)
        {
            return await _context.Bookings.FindAsync(id);
        }

        public async Task<Booking> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Car)
                    .ThenInclude(c => c.CarImages)
                .Include(b => b.User)
                .Include(b => b.DamageReports)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<Booking>> GetUserBookingsAsync(string userId)
        {
            return await _context.Bookings
                .Include(b => b.Car)
                    .ThenInclude(c => c.CarImages)
                .Where(b => b.UserId == userId && !b.IsDeleted)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.User)
                .Where(b => !b.IsDeleted)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetBookingsByStatusAsync(BookingStatus status)
        {
            return await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.User)
                .Where(b => b.Status == status && !b.IsDeleted)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> IsCarAvailableAsync(int carId, DateTime pickupDate, DateTime returnDate)
        {
            // التحقق من عدم وجود حجز متضارب
            var conflictingBooking = await _context.Bookings
                .Where(b => b.CarId == carId
                    && b.Status != BookingStatus.Cancelled
                    && b.Status != BookingStatus.Completed
                    && !b.IsDeleted
                    && (
                        (pickupDate >= b.PickupDate && pickupDate < b.ReturnDate) ||
                        (returnDate > b.PickupDate && returnDate <= b.ReturnDate) ||
                        (pickupDate <= b.PickupDate && returnDate >= b.ReturnDate)
                    ))
                .FirstOrDefaultAsync();

            return conflictingBooking == null;
        }

        public async Task<List<Booking>> GetActiveBookingsForCarAsync(int carId)
        {
            return await _context.Bookings
                .Where(b => b.CarId == carId
                    && b.Status == BookingStatus.Active
                    && !b.IsDeleted)
                .ToListAsync();
        }
    }
}
