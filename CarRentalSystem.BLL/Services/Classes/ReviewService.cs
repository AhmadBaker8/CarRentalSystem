using CarRentalSystem.BLL.Common;
using CarRentalSystem.BLL.Services.Interfaces;
using CarRentalSystem.DAL.DTO.Requests;
using CarRentalSystem.DAL.DTO.Responses;
using CarRentalSystem.DAL.Models;
using CarRentalSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.BLL.Services.Classes
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ICarRepository _carRepository;
        public ReviewService(IReviewRepository reviewRepository, IBookingRepository bookingRepository, ICarRepository carRepository)
        {
            _reviewRepository = reviewRepository;
            _bookingRepository = bookingRepository;
            _carRepository = carRepository;
        }


        // Customer Methods
        public async Task<ServiceResult<ReviewResponse>> AddReviewAsync(AddReviewRequset request, string userId)
        {
            try
            {
                // التحقق من الحجز
                var booking = await _bookingRepository.GetByIdAsync(request.BookingId);
                if (booking == null)
                {
                    return ServiceResult<ReviewResponse>.FailureResult(
                        "Booking not found",
                        new List<string> { "Booking not found" }
                    );
                }

                // التحقق من أن المستخدم هو مالك الحجز
                if (booking.UserId != userId)
                {
                    return ServiceResult<ReviewResponse>.FailureResult(
                        "Unauthorized",
                        new List<string> { "You can only review your own bookings" }
                    );
                }

                // التحقق من أن الحجز انتهى
                if (booking.Status != BookingStatus.Completed)
                {
                    return ServiceResult<ReviewResponse>.FailureResult(
                        "Cannot review",
                        new List<string> { "You can only review completed bookings" }
                    );
                }

                // التحقق من عدم وجود review سابق
                var existingReview = await _reviewRepository.GetByBookingIdAsync(request.BookingId);
                if (existingReview != null)
                {
                    return ServiceResult<ReviewResponse>.FailureResult(
                        "Already reviewed",
                        new List<string> { "You have already reviewed this booking" }
                    );
                }

                // إنشاء Review
                var review = new Review
                {
                    CarId = request.CarId,
                    BookingId = request.BookingId,
                    UserId = userId,
                    Rating = request.rating,
                    Comment = request.Comment,
                    IsVerified = true // Verified لأن المستخدم حجز فعلاً
                };

                var addedReview = await _reviewRepository.AddAsync(review);
                var response = MapToReviewResponse(addedReview);

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
                    return ServiceResult<bool>.SuccessResult(true, "Review deleted successfully");
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
                var response = reviews.Select(r => MapToReviewResponse(r)).ToList();

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
                    Reviews = reviews.Select(r => MapToReviewResponse(r)).ToList()
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

        // Admin Methods
        public async Task<ServiceResult<List<ReviewResponse>>> GetAllReviewsAsync()
        {
            try
            {
                var reviews = await _reviewRepository.GetAllReviewsAsync();
                var response = reviews.Select(r => MapToReviewResponse(r)).ToList();

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
                    return ServiceResult<bool>.SuccessResult(true, "Review deleted successfully");
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

        // Helper Method
        private ReviewResponse MapToReviewResponse(Review review)
        {
            return new ReviewResponse
            {
                Id = review.Id,
                CarId = review.CarId,
                BookingId = review.BookingId,
                UserId = review.UserId,
                UserName = review.User?.UserName ?? "Anonymous",
                UserFullName = review.User?.FullName ?? "Anonymous",
                Rating = review.Rating,
                Comment = review.Comment,
                IsVerified = review.IsVerified,
                CreatedAt = review.CreatedAt
            };
        }

    }
}
