using CarRentalSystem.BLL.Common;
using CarRentalSystem.BLL.Services.Interfaces;
using CarRentalSystem.DAL.DTO.Requests;
using CarRentalSystem.DAL.DTO.Responses;
using CarRentalSystem.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.BLL.Services.Classes
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;


        public AuthenticationService(UserManager<ApplicationUser> userManager, IConfiguration configuration, IEmailSender emailSender, SignInManager<ApplicationUser> SignInManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailSender = emailSender;
            _signInManager = SignInManager;
        }
        public async Task<ServiceResult<UserResponse>> LoginAsync(LoginRequest loginRequest)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginRequest.Email);
                if (user == null)
                {
                    return ServiceResult<UserResponse>.FailureResult(
                        "Login failed",
                        new List<string> { "Invalid email or password" }
                    );
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var userResponse = new UserResponse
                    {
                        Id = user.Id,
                        Email = user.Email,
                        UserName = user.UserName,
                        FullName = user.FullName,
                        Roles = roles.ToList(),
                        Token = await CreateTokenAsync(user),
                        TokenExpiration = DateTime.UtcNow.AddDays(15)
                    };

                    return ServiceResult<UserResponse>.SuccessResult(userResponse, "Login successful");
                }
                else if (result.IsLockedOut)
                {
                    return ServiceResult<UserResponse>.FailureResult(
                        "Account locked",
                        new List<string> { "Your account has been locked due to multiple failed login attempts. Please try again later." }
                    );
                }
                else if (result.IsNotAllowed)
                {
                    return ServiceResult<UserResponse>.FailureResult(
                        "Login not allowed",
                        new List<string> { "Please confirm your email before logging in." }
                    );
                }
                else
                {
                    return ServiceResult<UserResponse>.FailureResult(
                        "Login failed",
                        new List<string> { "Invalid email or password" }
                    );
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<UserResponse>.FailureResult(
                    "An error occurred during login",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<UserResponse>> RegisterAsync(RegisterRequest registerRequest, HttpRequest request)
        {
            try
            {

                // التحقق من وجود البريد الإلكتروني
                var existingUser = await _userManager.FindByEmailAsync(registerRequest.Email);
                if (existingUser != null)
                {
                    return ServiceResult<UserResponse>.FailureResult(
                        "Registration failed",
                        new List<string> { "Email is already registered" }
                    );
                }

                // التحقق من وجود اسم المستخدم
                var existingUsername = await _userManager.FindByNameAsync(registerRequest.UserName);
                if (existingUsername != null)
                {
                    return ServiceResult<UserResponse>.FailureResult(
                        "Registration failed",
                        new List<string> { "Username is already taken" }
                    );
                }

                var user = new ApplicationUser
                {
                    FullName = registerRequest.FullName,
                    Email = registerRequest.Email,
                    PhoneNumber = registerRequest.PhoneNumber,
                    UserName = registerRequest.UserName
                };

                var result = await _userManager.CreateAsync(user, registerRequest.Password);

                if (result.Succeeded)
                {
                    // إضافة الدور
                    await _userManager.AddToRoleAsync(user, "Customer");

                    // إرسال بريد التأكيد
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
                    var emailUrl = $"{request.Scheme}://{request.Host}/api/Identity/Account/ConfirmEmail?token={encodedToken}&userId={user.Id}";

                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "Confirm Your Email",
                        $"<h1>Welcome {user.UserName}!</h1>" +
                        $"<p>Please confirm your email by clicking the link below:</p>" +
                        $"<a href='{emailUrl}' style='padding: 10px 20px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px;'>Confirm Email</a>" +
                        $"<br><br><p>Or copy this link: {emailUrl}</p>"
                    );


                    var roles = await _userManager.GetRolesAsync(user);
                    var userResponse = new UserResponse
                    {
                        Id = user.Id,
                        Email = user.Email,
                        UserName = user.UserName,
                        FullName = user.FullName,
                        Roles = roles.ToList(),
                        Token = null,
                        TokenExpiration = DateTime.UtcNow.AddDays(15)
                    };

                    return ServiceResult<UserResponse>.SuccessResult(
                        userResponse,
                        "Registration successful. Please check your email to confirm your account."
                    );
                }

                var errors = result.Errors.Select(e => e.Description).ToList();
                return ServiceResult<UserResponse>.FailureResult("Registration failed", errors);
            }
            catch (Exception ex)
            {
                return ServiceResult<UserResponse>.FailureResult(
                    "An error occurred during registration",
                    new List<string> { ex.Message }
                );
            }
        }




        public async Task<ServiceResult<string>> ConfirmEmailAsync(string token, string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ServiceResult<string>.FailureResult(
                        "Confirmation failed",
                        new List<string> { "User not found" }
                    );
                }

                string decodedToken;
                try
                {
                    var tokenBytes = Convert.FromBase64String(token);
                    decodedToken = Encoding.UTF8.GetString(tokenBytes);
                }
                catch
                {
                    return ServiceResult<string>.FailureResult(
                        "Confirmation failed",
                        new List<string> { "Invalid token format" }
                    );
                }

                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

                if (result.Succeeded)
                {
                    return ServiceResult<string>.SuccessResult(
                        "Email confirmed successfully",
                        "Your email has been confirmed. You can now login."
                    );
                }

                var errors = result.Errors.Select(e => e.Description).ToList();
                return ServiceResult<string>.FailureResult("Email confirmation failed", errors);
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.FailureResult(
                    "An error occurred",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<bool>> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return ServiceResult<bool>.SuccessResult(
                        true,
                        "If your email exists, you will receive a password reset code."
                    );
                }

                
                var random = new Random();
                var code = random.Next(100000, 999999).ToString();

                user.CodeResetPassword = code;
                user.PasswordResetCodeExpiry = DateTime.UtcNow.AddMinutes(15);

                await _userManager.UpdateAsync(user);

                await _emailSender.SendEmailAsync(
                    request.Email,
                    "Password Reset Code",
                    $"<h2>Password Reset Request</h2>" +
                    $"<p>Your password reset code is: <strong>{code}</strong></p>" +
                    $"<p>This code will expire in 15 minutes.</p>" +
                    $"<p>If you didn't request this, please ignore this email.</p>"
                );

                return ServiceResult<bool>.SuccessResult(
                    true,
                    "If your email exists, you will receive a password reset code."
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult(
                    "An error occurred",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<bool>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return ServiceResult<bool>.FailureResult(
                        "Reset failed",
                        new List<string> { "User not found" }
                    );
                }

                if (user.CodeResetPassword != request.Code)
                {
                    return ServiceResult<bool>.FailureResult(
                        "Reset failed",
                        new List<string> { "Invalid reset code" }
                    );
                }

                if (user.PasswordResetCodeExpiry < DateTime.UtcNow)
                {
                    return ServiceResult<bool>.FailureResult(
                        "Reset failed",
                        new List<string> { "Reset code has expired. Please request a new one." }
                    );
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

                if (result.Succeeded)
                {
                    // مسح الكود بعد الاستخدام
                    user.CodeResetPassword = null;
                    user.PasswordResetCodeExpiry = null;
                    await _userManager.UpdateAsync(user);

                    await _emailSender.SendEmailAsync(
                        request.Email,
                        "Password Changed",
                        "<h2>Password Changed Successfully</h2>" +
                        "<p>Your password has been changed successfully.</p>" +
                        "<p>If you didn't make this change, please contact support immediately.</p>"
                    );

                    return ServiceResult<bool>.SuccessResult(true, "Password reset successfully");
                }

                var errors = result.Errors.Select(e => e.Description).ToList();
                return ServiceResult<bool>.FailureResult("Password reset failed", errors);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult(
                    "An error occurred",
                    new List<string> { ex.Message }
                );
            }
        }


        public async Task<ServiceResult<bool>> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ServiceResult<bool>.FailureResult(
                        "Change password failed",
                        new List<string> { "User not found" }
                    );
                }

                var result = await _userManager.ChangePasswordAsync(
                    user,
                    request.CurrentPassword,
                    request.NewPassword
                );

                if (result.Succeeded)
                {
                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "Password Changed",
                        "<h2>Password Changed</h2>" +
                        "<p>Your password has been changed successfully.</p>"
                    );

                    return ServiceResult<bool>.SuccessResult(true, "Password changed successfully");
                }

                var errors = result.Errors.Select(e => e.Description).ToList();
                return ServiceResult<bool>.FailureResult("Password change failed", errors);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult(
                    "An error occurred",
                    new List<string> { ex.Message }
                );
            }
        }


        public async Task<ServiceResult<ProfileResponse>> GetProfileAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ServiceResult<ProfileResponse>.FailureResult(
                        "User not found",
                        new List<string> { "User not found" }
                    );
                }

                var profile = new ProfileResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    City = user.City,
                    IsEmailConfirmed = user.EmailConfirmed
                };

                return ServiceResult<ProfileResponse>.SuccessResult(profile, "Profile retrieved successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<ProfileResponse>.FailureResult(
                    "An error occurred",
                    new List<string> { ex.Message }
                );
            }
        }


        public async Task<ServiceResult<ProfileResponse>> UpdateProfileAsync(string userId, UpdateProfileRequest request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ServiceResult<ProfileResponse>.FailureResult(
                        "User not found",
                        new List<string> { "User not found" }
                    );
                }

                user.FullName = request.FullName;
                user.PhoneNumber = request.PhoneNumber;
                user.City = request.City;
                user.PhoneNumber = request.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    var profile = new ProfileResponse
                    {
                        Id = user.Id,
                        Email = user.Email,
                        UserName = user.UserName,
                        FullName = user.FullName,
                        PhoneNumber = user.PhoneNumber,
                        City = user.City,
                        IsEmailConfirmed = user.EmailConfirmed
                    };

                    return ServiceResult<ProfileResponse>.SuccessResult(profile, "Profile updated successfully");
                }

                var errors = result.Errors.Select(e => e.Description).ToList();
                return ServiceResult<ProfileResponse>.FailureResult("Profile update failed", errors);
            }
            catch (Exception ex)
            {
                return ServiceResult<ProfileResponse>.FailureResult(
                    "An error occurred",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<bool>> LogoutAsync(string userId)
        {
            try
            {
                await _signInManager.SignOutAsync();
                return ServiceResult<bool>.SuccessResult(true, "Logout successful");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult(
                    "Logout failed",
                    new List<string> { ex.Message }
                );
            }
        }


        private async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("jwtOptions")["SecretKey"])
            );
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration.GetSection("jwtOptions")["Issuer"],
                audience: _configuration.GetSection("jwtOptions")["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(15),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
