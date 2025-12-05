using CarRentalSystem.BLL.Common;
using CarRentalSystem.BLL.Services.Interfaces;
using CarRentalSystem.DAL.DTO.Requests;
using CarRentalSystem.DAL.DTO.Responses;
using CarRentalSystem.DAL.Models;
using CarRentalSystem.DAL.Repositories.Interfaces;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRentalSystem.BLL.Services.Classes
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ICarRepository _carRepository;

        public ReviewService(
            IReviewRepository reviewRepository,
            IBookingRepository bookingRepository,
            ICarRepository carRepository)
        {
            _reviewRepository = reviewRepository;
            _bookingRepository = bookingRepository;
            _carRepository = carRepository;
        }

        // ============================
        //   Customer Methods
        // ============================
        public async Task<ServiceResult<ReviewResponse>> AddReviewAsync(AddReviewRequset request, string userId)
        {
            try
            {
                // 1) التحقق من الحجز
                var booking = await _bookingRepository.GetByIdAsync(request.BookingId);
                if (booking == null)
                {
                    return ServiceResult<ReviewResponse>.FailureResult(
                        "Booking not found",
                        new List<string> { "Booking not found" }
                    );
                }

                // 2) التحقق من أن المستخدم هو مالك الحجز
                if (booking.UserId != userId)
                {
                    return ServiceResult<ReviewResponse>.FailureResult(
                        "Unauthorized",
                        new List<string> { "You can only review your own bookings" }
                    );
                }

                // 3) التحقق من أن الحجز مكتمل
                if (booking.Status != BookingStatus.Completed)
                {
                    return ServiceResult<ReviewResponse>.FailureResult(
                        "Cannot review",
                        new List<string> { "You can only review completed bookings" }
                    );
                }

                // 4) التحقق أن CarId في الطلب يطابق CarId تبع الحجز
                if (booking.CarId != request.CarId)
                {
                    return ServiceResult<ReviewResponse>.FailureResult(
                        "Invalid car",
                        new List<string> { "Selected car does not match the booking car" }
                    );
                }

                // 5) التحقق من عدم وجود Review سابق لهذا الحجز
                var existingReview = await _reviewRepository.GetByBookingIdAsync(request.BookingId);
                if (existingReview != null)
                {
                    return ServiceResult<ReviewResponse>.FailureResult(
                        "Already reviewed",
                        new List<string> { "You have already reviewed this booking" }
                    );
                }

                // 6) إنشاء Review
                var review = new Review
                {
                    CarId = request.CarId,
                    BookingId = request.BookingId,
                    UserId = userId,
                    Rating = request.rating,
                    Comment = request.Comment,
                    IsVerified = true // Verified لأن المستخدم فعلاً حجز السيارة
                };

                var addedReview = await _reviewRepository.AddAsync(review);

                // AddAsync يرجع Review مع Include(User, Car, Booking)
                var response = addedReview.Adapt<ReviewResponse>();

                return ServiceResult<ReviewResponse>.SuccessResult(
                    response,
                    "Review added successfully"
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<ReviewResponse>.FailureResult(
                    "Failed to add review",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<bool>> DeleteReviewAsync(int reviewId, string userId)
        {
            try
            {
                var review = await _reviewRepository.GetByIdAsync(reviewId);
                if (review == null)
                {
                    return ServiceResult<bool>.FailureResult(
                        "Review not found",
                        new List<string> { "Review not found" }
                    );
                }

                if (review.UserId != userId)
                {
                    return ServiceResult<bool>.FailureResult(
                        "Unauthorized",
                        new List<string> { "You can only delete your own reviews" }
                    );
                }

                var result = await _reviewRepository.DeleteAsync(reviewId);

                if (result)
                {
                    return ServiceResult<bool>.SuccessResult(
                        true,
                        "Review deleted successfully"
                    );
                }

                return ServiceResult<bool>.FailureResult(
                    "Failed to delete review",
                    new List<string> { "Failed to delete review" }
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult(
                    "Failed to delete review",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<List<ReviewResponse>>> GetMyReviewsAsync(string userId)
        {
            try
            {
                var reviews = await _reviewRepository.GetUserReviewsAsync(userId);

                var response = reviews.Adapt<List<ReviewResponse>>();

                return ServiceResult<List<ReviewResponse>>.SuccessResult(
                    response,
                    "Reviews retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<List<ReviewResponse>>.FailureResult(
                    "Failed to retrieve reviews",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<CarReviewsResponse>> GetCarReviewsAsync(int carId)
        {
            try
            {
                var car = await _carRepository.GetByIdAsync(carId);
                if (car == null)
                {
                    return ServiceResult<CarReviewsResponse>.FailureResult(
                        "Car not found",
                        new List<string> { "Car not found" }
                    );
                }

                var reviews = await _reviewRepository.GetCarReviewsAsync(carId);
                var averageRating = await _reviewRepository.GetAverageRatingAsync(carId);

                var response = new CarReviewsResponse
                {
                    CarId = carId,
                    CarName = $"{car.Make} {car.Model}",
                    AverageRating = averageRating,
                    TotalReviews = reviews.Count,
                    Reviews = reviews.Adapt<List<ReviewResponse>>()
                };

                return ServiceResult<CarReviewsResponse>.SuccessResult(
                    response,
                    "Car reviews retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<CarReviewsResponse>.FailureResult(
                    "Failed to retrieve reviews",
                    new List<string> { ex.Message }
                );
            }
        }

        // ============================
        //   Admin Methods
        // ============================
        public async Task<ServiceResult<List<ReviewResponse>>> GetAllReviewsAsync()
        {
            try
            {
                var reviews = await _reviewRepository.GetAllReviewsAsync();

                var response = reviews.Adapt<List<ReviewResponse>>();

                return ServiceResult<List<ReviewResponse>>.SuccessResult(
                    response,
                    "All reviews retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<List<ReviewResponse>>.FailureResult(
                    "Failed to retrieve reviews",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<bool>> DeleteReviewAdminAsync(int reviewId)
        {
            try
            {
                var result = await _reviewRepository.DeleteAsync(reviewId);

                if (result)
                {
                    return ServiceResult<bool>.SuccessResult(
                        true,
                        "Review deleted successfully"
                    );
                }

                return ServiceResult<bool>.FailureResult(
                    "Review not found",
                    new List<string> { "Review not found" }
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult(
                    "Failed to delete review",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}
