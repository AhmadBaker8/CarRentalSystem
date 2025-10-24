using CarRentalSystem.BLL.Common;
using CarRentalSystem.DAL.DTO.Requests;
using CarRentalSystem.DAL.DTO.Responses;
using CarRentalSystem.DAL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.BLL.Services.Interfaces
{
    public interface IBookingService
    {
        // Customer
        Task<ServiceResult<BookingResponse>> CreateBookingAsync(CreateBookingRequest request, string userId);
        Task<ServiceResult<List<BookingSummaryResponse>>> GetUserBookingsAsync(string userId, HttpRequest httpRequest = null);
        Task<ServiceResult<BookingResponse>> GetBookingDetailsAsync(int bookingId, string userId, HttpRequest httpRequest = null);
        Task<ServiceResult<bool>> CancelBookingAsync(int bookingId, string userId);
        Task<ServiceResult<bool>> IsCarAvailableAsync(int carId, DateTime pickupDate, DateTime returnDate);
        Task<ServiceResult<decimal>> CalculateBookingPriceAsync(int carId, DateTime pickupDate, DateTime returnDate, bool hasInsurance, bool needsGPS, bool needsChildSeat);

        // Admin
        Task<ServiceResult<List<BookingResponse>>> GetAllBookingsAsync(HttpRequest httpRequest = null);
        Task<ServiceResult<List<BookingResponse>>> GetBookingsByStatusAsync(BookingStatus status, HttpRequest httpRequest = null);
        Task<ServiceResult<bool>> UpdateBookingStatusAsync(int bookingId, UpdateBookingStatusRequest request);
        Task<ServiceResult<bool>> MarkAsPickedUpAsync(int bookingId, int pickupMileage);
        Task<ServiceResult<bool>> ProcessReturnAsync(int bookingId, int returnMileage, string returnConditionNotes);
    }
}
