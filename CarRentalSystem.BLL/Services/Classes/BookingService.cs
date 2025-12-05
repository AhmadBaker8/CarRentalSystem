using CarRentalSystem.BLL.Common;
using CarRentalSystem.BLL.Services.Interfaces;
using CarRentalSystem.DAL.DTO.Requests;
using CarRentalSystem.DAL.DTO.Responses;
using CarRentalSystem.DAL.Models;
using CarRentalSystem.DAL.Repositories.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRentalSystem.BLL.Services.Classes
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICarRepository _carRepository;

        // Prices for extras
        private const decimal INSURANCE_DAILY_PRICE = 15m;
        private const decimal GPS_DAILY_PRICE = 5m;

        public BookingService(IBookingRepository bookingRepository, ICarRepository carRepository)
        {
            _bookingRepository = bookingRepository;
            _carRepository = carRepository;
        }

        // ============================
        //   Customer Methods
        // ============================
        public async Task<ServiceResult<BookingResponse>> CreateBookingAsync(CreateBookingRequest request, string userId)
        {
            try
            {
                // 1. التحقق من التواريخ
                if (request.PickupDate >= request.ReturnDate)
                {
                    return ServiceResult<BookingResponse>.FailureResult(
                        "Invalid dates",
                        new List<string> { "Return date must be after pickup date" }
                    );
                }

                if (request.PickupDate < DateTime.UtcNow)
                {
                    return ServiceResult<BookingResponse>.FailureResult(
                        "Invalid pickup date",
                        new List<string> { "Pickup date cannot be in the past" }
                    );
                }

                // 2. التحقق من توفر السيارة
                var isAvailable = await _bookingRepository.IsCarAvailableAsync(
                    request.CarId,
                    request.PickupDate,
                    request.ReturnDate
                );

                if (!isAvailable)
                {
                    return ServiceResult<BookingResponse>.FailureResult(
                        "Car not available",
                        new List<string> { "Car is not available for the selected dates" }
                    );
                }

                // 3. جلب السيارة
                var car = await _carRepository.GetByIdAsync(request.CarId);
                if (car == null)
                {
                    return ServiceResult<BookingResponse>.FailureResult(
                        "Car not found",
                        new List<string> { "Car not found" }
                    );
                }

                if (car.Status != CarStatus.Available)
                {
                    return ServiceResult<BookingResponse>.FailureResult(
                        "Car not available",
                        new List<string> { "Car is not available for rental" }
                    );
                }

                // 4. حساب السعر
                var rentalDays = (request.ReturnDate - request.PickupDate).Days;
                var basePrice = CalculateBasePrice(car, rentalDays);
                var insurancePrice = request.HasInsurance ? (rentalDays * INSURANCE_DAILY_PRICE) : 0;
                var gpsPrice = request.NeedsGPS ? (rentalDays * GPS_DAILY_PRICE) : 0;
                var totalPrice = basePrice + insurancePrice + gpsPrice;

                // 5. إنشاء الحجز
                var booking = new Booking
                {
                    CarId = request.CarId,
                    UserId = userId,
                    PickupDate = request.PickupDate,
                    ReturnDate = request.ReturnDate,
                    PickupLocation = request.PickupLocation,
                    ReturnLocation = request.ReturnLocation,
                    RentalDays = rentalDays,
                    BasePrice = basePrice,
                    InsurancePrice = insurancePrice,
                    AdditionalCharges = gpsPrice,
                    TotalPrice = totalPrice,
                    Status = BookingStatus.Pending,
                    PaymentStatus = PaymentStatus.Pending,
                    HasInsurance = request.HasInsurance,
                    NeedsGPS = request.NeedsGPS,
                    SpecialRequests = request.SpecialRequests,
                    DriverName = request.DriverName,
                    DriverLicenseNumber = request.DriverLicenseNumber,
                    DriverLicenseExpiry = request.DriverLicenseExpiry,
                    ContactPhone = request.ContactPhone
                };

                var addedBooking = await _bookingRepository.AddAsync(booking);

                // نجيب الحجز مع التفاصيل (سيارة + صور + مستخدم) عشان Mapster يشتغل صح
                var fullBooking = await _bookingRepository.GetByIdWithDetailsAsync(addedBooking.Id);

                var response = fullBooking.Adapt<BookingResponse>();
                SetFullImageUrl(response, null); // هنا ما معنا HttpRequest، فنرجع الـ path كما هو

                return ServiceResult<BookingResponse>.SuccessResult(
                    response,
                    "Booking created successfully"
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<BookingResponse>.FailureResult(
                    "Failed to create booking",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<List<BookingSummaryResponse>>> GetUserBookingsAsync(string userId, HttpRequest httpRequest = null)
        {
            try
            {
                var bookings = await _bookingRepository.GetUserBookingsAsync(userId);

                var response = bookings.Adapt<List<BookingSummaryResponse>>();

                return ServiceResult<List<BookingSummaryResponse>>.SuccessResult(
                    response,
                    "Bookings retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<List<BookingSummaryResponse>>.FailureResult(
                    "Failed to retrieve bookings",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<BookingResponse>> GetBookingDetailsAsync(int bookingId, string userId, HttpRequest httpRequest = null)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);

                if (booking == null)
                {
                    return ServiceResult<BookingResponse>.FailureResult(
                        "Booking not found",
                        new List<string> { "Booking not found" }
                    );
                }

                if (booking.UserId != userId)
                {
                    return ServiceResult<BookingResponse>.FailureResult(
                        "Unauthorized",
                        new List<string> { "You don't have access to this booking" }
                    );
                }

                var response = booking.Adapt<BookingResponse>();
                SetFullImageUrl(response, httpRequest);

                return ServiceResult<BookingResponse>.SuccessResult(
                    response,
                    "Booking details retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<BookingResponse>.FailureResult(
                    "Failed to retrieve booking details",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<bool>> CancelBookingAsync(int bookingId, string userId)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdAsync(bookingId);

                if (booking == null)
                {
                    return ServiceResult<bool>.FailureResult(
                        "Booking not found",
                        new List<string> { "Booking not found" }
                    );
                }

                if (booking.UserId != userId)
                {
                    return ServiceResult<bool>.FailureResult(
                        "Unauthorized",
                        new List<string> { "You don't have access to this booking" }
                    );
                }

                // يسمح بالإلغاء فقط لو Pending أو Confirmed
                if (booking.Status != BookingStatus.Pending && booking.Status != BookingStatus.Confirmed)
                {
                    return ServiceResult<bool>.FailureResult(
                        "Cannot cancel booking",
                        new List<string> { "Booking cannot be cancelled at this stage" }
                    );
                }

                booking.Status = BookingStatus.Cancelled;
                await _bookingRepository.DeleteAsync(bookingId);

                return ServiceResult<bool>.SuccessResult(true, "Booking cancelled successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult(
                    "Failed to cancel booking",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<bool>> IsCarAvailableAsync(int carId, DateTime pickupDate, DateTime returnDate)
        {
            try
            {
                var isAvailable = await _bookingRepository.IsCarAvailableAsync(carId, pickupDate, returnDate);

                return ServiceResult<bool>.SuccessResult(
                    isAvailable,
                    isAvailable ? "Car is available" : "Car is not available"
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult(
                    "Failed to check availability",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<decimal>> CalculateBookingPriceAsync(
            int carId,
            DateTime pickupDate,
            DateTime returnDate,
            bool hasInsurance,
            bool needsGPS,
            bool needsChildSeat)
        {
            try
            {
                var car = await _carRepository.GetByIdAsync(carId);
                if (car == null)
                {
                    return ServiceResult<decimal>.FailureResult(
                        "Car not found",
                        new List<string> { "Car not found" }
                    );
                }

                var rentalDays = (returnDate - pickupDate).Days;
                var basePrice = CalculateBasePrice(car, rentalDays);
                var insurancePrice = hasInsurance ? (rentalDays * INSURANCE_DAILY_PRICE) : 0;
                var gpsPrice = needsGPS ? (rentalDays * GPS_DAILY_PRICE) : 0;
                var totalPrice = basePrice + insurancePrice + gpsPrice;

                return ServiceResult<decimal>.SuccessResult(
                    totalPrice,
                    "Price calculated successfully"
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<decimal>.FailureResult(
                    "Failed to calculate price",
                    new List<string> { ex.Message }
                );
            }
        }

        // ============================
        //   Admin Methods
        // ============================
        public async Task<ServiceResult<List<BookingResponse>>> GetAllBookingsAsync(HttpRequest httpRequest = null)
        {
            try
            {
                var bookings = await _bookingRepository.GetAllBookingsAsync();

                var response = bookings.Adapt<List<BookingResponse>>();
                SetFullImageUrls(response, httpRequest);

                return ServiceResult<List<BookingResponse>>.SuccessResult(
                    response,
                    "Bookings retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<List<BookingResponse>>.FailureResult(
                    "Failed to retrieve bookings",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<List<BookingResponse>>> GetBookingsByStatusAsync(BookingStatus status, HttpRequest httpRequest = null)
        {
            try
            {
                var bookings = await _bookingRepository.GetBookingsByStatusAsync(status);

                var response = bookings.Adapt<List<BookingResponse>>();
                SetFullImageUrls(response, httpRequest);

                return ServiceResult<List<BookingResponse>>.SuccessResult(
                    response,
                    $"Bookings with status {status} retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<List<BookingResponse>>.FailureResult(
                    "Failed to retrieve bookings",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<bool>> UpdateBookingStatusAsync(int bookingId, UpdateBookingStatusRequest request)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdAsync(bookingId);

                if (booking == null)
                {
                    return ServiceResult<bool>.FailureResult(
                        "Booking not found",
                        new List<string> { "Booking not found" }
                    );
                }

                booking.Status = request.Status;
                await _bookingRepository.UpdateAsync(booking);

                return ServiceResult<bool>.SuccessResult(true, "Booking status updated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult(
                    "Failed to update booking status",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<bool>> MarkAsPickedUpAsync(int bookingId, int pickupMileage)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdAsync(bookingId);

                if (booking == null)
                {
                    return ServiceResult<bool>.FailureResult(
                        "Booking not found",
                        new List<string> { "Booking not found" }
                    );
                }

                booking.Status = BookingStatus.Active;
                booking.PickupMileage = pickupMileage;
                await _bookingRepository.UpdateAsync(booking);

                return ServiceResult<bool>.SuccessResult(true, "Booking marked as picked up");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult(
                    "Failed to mark booking as picked up",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<bool>> ProcessReturnAsync(int bookingId, int returnMileage, string returnConditionNotes)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdAsync(bookingId);

                if (booking == null)
                {
                    return ServiceResult<bool>.FailureResult(
                        "Booking not found",
                        new List<string> { "Booking not found" }
                    );
                }

                booking.Status = BookingStatus.Completed;
                booking.ActualReturnDate = DateTime.UtcNow;
                booking.ReturnMileage = returnMileage;
                booking.ReturnConditionNotes = returnConditionNotes;

                // حساب غرامة التأخير إن وجدت
                if (DateTime.UtcNow > booking.ReturnDate)
                {
                    var car = await _carRepository.GetByIdAsync(booking.CarId);
                    var lateHours = (DateTime.UtcNow - booking.ReturnDate).TotalHours;
                    var lateDays = Math.Ceiling(lateHours / 24);
                    booking.LateFee = (decimal)lateDays * car.DailyRate * 1.5m;
                    booking.TotalPrice += booking.LateFee;
                }

                await _bookingRepository.UpdateAsync(booking);

                return ServiceResult<bool>.SuccessResult(true, "Booking return processed successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult(
                    "Failed to process return",
                    new List<string> { ex.Message }
                );
            }
        }

        // ============================
        //   Helper Methods
        // ============================
        private decimal CalculateBasePrice(Car car, int rentalDays)
        {
            if (rentalDays >= 30)
                return rentalDays * car.MonthlyRate > 0
                    ? car.MonthlyRate
                    : (rentalDays * car.DailyRate * 0.7m);

            if (rentalDays >= 7)
                return rentalDays * car.WeeklyRate > 0
                    ? car.WeeklyRate
                    : (rentalDays * car.DailyRate * 0.85m);

            return rentalDays * car.DailyRate;
        }

        private static string BuildImageUrl(string imagePath, HttpRequest httpRequest)
        {
            if (string.IsNullOrEmpty(imagePath))
                return string.Empty;

            if (httpRequest == null)
                return imagePath; // relative

            return $"{httpRequest.Scheme}://{httpRequest.Host}/images/{imagePath}";
        }

        private static void SetFullImageUrl(BookingResponse booking, HttpRequest httpRequest)
        {
            if (booking == null || httpRequest == null)
                return;

            if (!string.IsNullOrEmpty(booking.CarImage))
            {
                booking.CarImage = BuildImageUrl(booking.CarImage, httpRequest);
            }
        }

        private static void SetFullImageUrls(IEnumerable<BookingResponse> bookings, HttpRequest httpRequest)
        {
            if (bookings == null || httpRequest == null)
                return;

            foreach (var booking in bookings)
            {
                if (!string.IsNullOrEmpty(booking.CarImage))
                {
                    booking.CarImage = BuildImageUrl(booking.CarImage, httpRequest);
                }
            }
        }
    }
}
