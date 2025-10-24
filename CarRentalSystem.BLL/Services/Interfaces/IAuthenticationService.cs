using CarRentalSystem.BLL.Common;
using CarRentalSystem.DAL.DTO.Requests;
using CarRentalSystem.DAL.DTO.Responses;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.BLL.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ServiceResult<UserResponse>> RegisterAsync(RegisterRequest registerRequest, HttpRequest request);
        Task<ServiceResult<UserResponse>> LoginAsync(LoginRequest loginRequest);
        Task<ServiceResult<string>> ConfirmEmailAsync(string token, string userId);
        Task<ServiceResult<bool>> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<ServiceResult<bool>> ResetPasswordAsync(ResetPasswordRequest request);
        Task<ServiceResult<bool>> ChangePasswordAsync(string userId, ChangePasswordRequest request);
        Task<ServiceResult<ProfileResponse>> GetProfileAsync(string userId);
        Task<ServiceResult<ProfileResponse>> UpdateProfileAsync(string userId, UpdateProfileRequest request);
        Task<ServiceResult<bool>> LogoutAsync(string userId);
    }
}
