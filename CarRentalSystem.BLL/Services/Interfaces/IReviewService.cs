using CarRentalSystem.BLL.Common;
using CarRentalSystem.DAL.DTO.Requests;
using CarRentalSystem.DAL.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.BLL.Services.Interfaces
{
    public interface IReviewService
    {
        // Customer
        Task<ServiceResult<ReviewResponse>> AddReviewAsync(AddReviewRequset request, string userId);
        Task<ServiceResult<bool>> DeleteReviewAsync(int reviewId, string userId);
        Task<ServiceResult<List<ReviewResponse>>> GetMyReviewsAsync(string userId);

        Task<ServiceResult<CarReviewsResponse>> GetCarReviewsAsync(int carId);

        // Admin
        Task<ServiceResult<List<ReviewResponse>>> GetAllReviewsAsync();
        Task<ServiceResult<bool>> DeleteReviewAdminAsync(int reviewId);
    }
}
