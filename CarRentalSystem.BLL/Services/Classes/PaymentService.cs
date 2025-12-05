using CarRentalSystem.BLL.Common;
using CarRentalSystem.BLL.Services.Interfaces;
using CarRentalSystem.DAL.DTO.Requests;
using CarRentalSystem.DAL.DTO.Responses;
using CarRentalSystem.DAL.Models;
using CarRentalSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRentalSystem.BLL.Services.Classes
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IBookingRepository _bookingRepository;
        private readonly IEmailSender _emailSender;

        public PaymentService(
            IConfiguration configuration,
            IBookingRepository bookingRepository,
            IEmailSender emailSender)
        {
            _configuration = configuration;
            _bookingRepository = bookingRepository;
            _emailSender = emailSender;

            // تعيين Stripe API Key
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }

        public async Task<ServiceResult<PaymentResponse>> CreatePaymentSessionAsync(
            PaymentRequest request,
            HttpRequest httpRequest)
        {
            try
            {
                // 1) التحقق من الحجز + جلب التفاصيل (User + Car) للإيميل لو دفع كاش
                var booking = await _bookingRepository.GetByIdWithDetailsAsync(request.BookingId);
                if (booking == null)
                {
                    return ServiceResult<PaymentResponse>.FailureResult(
                        "Booking not found",
                        new List<string> { "Booking not found" }
                    );
                }

                // 2) التحقق من عدم الدفع مسبقاً
                if (booking.PaymentStatus == PaymentStatus.Paid)
                {
                    return ServiceResult<PaymentResponse>.FailureResult(
                        "Already paid",
                        new List<string> { "This booking has already been paid" }
                    );
                }

                // 3) التحقق من مبلغ الدفع (ما نعتمد على قيمة جايه من الـ Frontend)
                if (request.Amount != booking.TotalPrice)
                {
                    return ServiceResult<PaymentResponse>.FailureResult(
                        "Invalid amount",
                        new List<string> { "Payment amount does not match booking total price" }
                    );
                }

                // 4) الدفع كاش
                if (request.PaymentMethod == PaymentMethodEnum.Cash)
                {
                    booking.PaymentStatus = PaymentStatus.Pending; // بيدفع عند الاستلام
                    booking.Status = BookingStatus.Confirmed;
                    await _bookingRepository.UpdateAsync(booking);

                    var paymentResponse = new PaymentResponse
                    {
                        Success = true,
                        Message = "Booking confirmed. Pay on pickup."
                    };

                    // إرسال إيميل تأكيد الحجز
                    await SendBookingConfirmationEmailAsync(booking, "Cash on Pickup");

                    return ServiceResult<PaymentResponse>.SuccessResult(
                        paymentResponse,
                        "Booking confirmed"
                    );
                }

                // 5) الدفع Visa (Stripe)
                if (request.PaymentMethod == PaymentMethodEnum.Visa)
                {
                    var options = new SessionCreateOptions
                    {
                        PaymentMethodTypes = new List<string> { "card" },
                        LineItems = new List<SessionLineItemOptions>
                        {
                            new SessionLineItemOptions
                            {
                                PriceData = new SessionLineItemPriceDataOptions
                                {
                                    Currency = "usd",
                                    ProductData = new SessionLineItemPriceDataProductDataOptions
                                    {
                                        Name = request.BookingDescription,
                                        Description = $"Car Rental - Booking #{request.BookingId}"
                                    },
                                    UnitAmount = (long)(request.Amount * 100) // تحويل لـ cents
                                },
                                Quantity = 1
                            }
                        },
                        Mode = "payment",
                        SuccessUrl = $"{httpRequest.Scheme}://{httpRequest.Host}/api/Customer/Payments/success?sessionId={{CHECKOUT_SESSION_ID}}&bookingId={request.BookingId}",
                        CancelUrl = $"{httpRequest.Scheme}://{httpRequest.Host}/api/Customer/Payments/cancel?bookingId={request.BookingId}",
                        ClientReferenceId = request.BookingId.ToString()
                    };

                    var service = new SessionService();
                    var session = await service.CreateAsync(options);

                    // حفظ Session ID في الحجز
                    booking.PaymentId = session.Id;
                    await _bookingRepository.UpdateAsync(booking);

                    var response = new PaymentResponse
                    {
                        SessionId = session.Id,
                        Success = true,
                        Message = "Payment session created successfully"
                    };

                    return ServiceResult<PaymentResponse>.SuccessResult(
                        response,
                        "Redirect to Stripe checkout"
                    );
                }

                return ServiceResult<PaymentResponse>.FailureResult(
                    "Invalid payment method",
                    new List<string> { "Payment method not supported" }
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<PaymentResponse>.FailureResult(
                    "Failed to create payment session",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<bool>> HandlePaymentSuccessAsync(int bookingId, string sessionId)
        {
            try
            {
                // نجيب الحجز مع التفاصيل عشان الإيميل (User + Car)
                var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);
                if (booking == null)
                {
                    return ServiceResult<bool>.FailureResult(
                        "Booking not found",
                        new List<string> { "Booking not found" }
                    );
                }

                // التحقق من Session من Stripe
                var service = new SessionService();
                var session = await service.GetAsync(sessionId);

                if (session.PaymentStatus == "paid")
                {
                    booking.PaymentStatus = PaymentStatus.Paid;
                    booking.Status = BookingStatus.Confirmed;
                    await _bookingRepository.UpdateAsync(booking);

                    await SendBookingConfirmationEmailAsync(booking, "Credit Card");

                    return ServiceResult<bool>.SuccessResult(
                        true,
                        "Payment processed successfully"
                    );
                }

                return ServiceResult<bool>.FailureResult(
                    "Payment not completed",
                    new List<string> { "Payment was not completed" }
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult(
                    "Failed to process payment success",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<bool>> HandlePaymentCancelAsync(int bookingId)
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

                booking.PaymentStatus = PaymentStatus.Failed;
                await _bookingRepository.UpdateAsync(booking);

                return ServiceResult<bool>.SuccessResult(
                    true,
                    "Payment cancelled"
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult(
                    "Failed to cancel payment",
                    new List<string> { ex.Message }
                );
            }
        }

        private async Task SendBookingConfirmationEmailAsync(Booking booking, string paymentMethod)
        {
            var subject = paymentMethod == "Cash on Pickup"
                ? "Booking Confirmed - Pay on Pickup"
                : "Payment Successful - Booking Confirmed";

            var body = $@"
            <h1>Booking Confirmation</h1>
            <p>Dear {booking.User?.FullName},</p>
            <p>Thank you for your booking!</p>
            <hr/>
            <h2>Booking Details</h2>
            <p><strong>Booking ID:</strong> #{booking.Id}</p>
            <p><strong>Car:</strong> {booking.Car?.Make} {booking.Car?.Model}</p>
            <p><strong>Pickup Date:</strong> {booking.PickupDate:dd/MM/yyyy HH:mm}</p>
            <p><strong>Return Date:</strong> {booking.ReturnDate:dd/MM/yyyy HH:mm}</p>
            <p><strong>Total Amount:</strong> ${booking.TotalPrice}</p>
            <p><strong>Payment Method:</strong> {paymentMethod}</p>
            <hr/>
            <p>Pickup Location: {booking.PickupLocation}</p>
            <p>Contact: {booking.ContactPhone}</p>
            <hr/>
            {(paymentMethod == "Cash on Pickup"
                ? "<p style='color: red;'><strong>Please bring payment upon pickup.</strong></p>"
                : "<p style='color: green;'><strong>Payment has been received.</strong></p>")}
            <p>Thank you for choosing us!</p>
        ";

            await _emailSender.SendEmailAsync(booking.User?.Email, subject, body);
        }
    }
}
