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
    public interface IPaymentService
    {
        Task<ServiceResult<PaymentResponse>> CreatePaymentSessionAsync(PaymentRequest request, HttpRequest httpRequest);
        Task<ServiceResult<bool>> HandlePaymentSuccessAsync(int bookingId, string sessionId);
        Task<ServiceResult<bool>> HandlePaymentCancelAsync(int bookingId);
    }
}
