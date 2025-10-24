using CarRentalSystem.BLL.Services.Interfaces;
using CarRentalSystem.DAL.DTO.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Areas.Customer
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // POST: api/Customer/Payments
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _paymentService.CreatePaymentSessionAsync(request,Request);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        // GET: api/Customer/Payments/success
        [HttpGet("success")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentSuccess([FromQuery] string sessionId, [FromQuery] int bookingId)
        {
            var result = await _paymentService.HandlePaymentSuccessAsync(bookingId, sessionId);

            if (result.Success)
                return Ok(new { message = "Payment successful! Your booking is confirmed." });

            return BadRequest(result);
        }

        // GET: api/Customer/Payments/cancel
        [HttpGet("cancel")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentCancel([FromQuery] int bookingId)
        {
            var result = await _paymentService.HandlePaymentCancelAsync(bookingId);

            if (result.Success)
                return Ok(new { message = "Payment cancelled. You can try again." });

            return BadRequest(result);
        }
    }
}
