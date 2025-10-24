using CarRentalSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking> AddAsync(Booking booking);
        Task<int> UpdateAsync(Booking booking);
        Task<bool> DeleteAsync(int id);
        Task<Booking> GetByIdAsync(int id);
        Task<Booking> GetByIdWithDetailsAsync(int id);
        Task<List<Booking>> GetUserBookingsAsync(string userId);
        Task<List<Booking>> GetAllBookingsAsync();
        Task<List<Booking>> GetBookingsByStatusAsync(BookingStatus status);
        Task<bool> IsCarAvailableAsync(int carId, DateTime pickupDate, DateTime returnDate);
        Task<List<Booking>> GetActiveBookingsForCarAsync(int carId);
    }
}
