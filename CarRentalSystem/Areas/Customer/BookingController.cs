using CarRentalSystem.BLL.Services.Interfaces;
using CarRentalSystem.DAL.DTO.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarRentalSystem.Areas.Customer
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Customer")]
    //[Authorize(Roles = "Customer")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }


        // POST: api/Customer/Bookings
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _bookingService.CreateBookingAsync(request, userId);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        // GET: api/Customer/Bookings
        [HttpGet]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _bookingService.GetUserBookingsAsync(userId, Request);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        // GET: api/Customer/Bookings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingDetails(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _bookingService.GetBookingDetailsAsync(id, userId, Request);

            if (result.Success)
                return Ok(result);

            return result.Message == "Booking not found" ? NotFound(result) : BadRequest(result);
        }

        // DELETE: api/Customer/Bookings/5/cancel
        [HttpDelete("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _bookingService.CancelBookingAsync(id, userId);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        // POST: api/Customer/Bookings/check-availability
        [HttpPost("check-availability")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckAvailability([FromBody] CheckAvailabilityRequest request)
        {
            var result = await _bookingService.IsCarAvailableAsync(
                request.CarId,
                request.pickupDate,
                request.ReturnDate
            );

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        // POST: api/Customer/Bookings/calculate-price
        [HttpPost("calculate-price")]
        [AllowAnonymous]
        public async Task<IActionResult> CalculatePrice([FromBody] CalculatePriceRequest request)
        {
            
            var result = await _bookingService.CalculateBookingPriceAsync(
                request.CarId,
                request.PickupDate,
                request.ReturnDate,
                request.HasInsurance,
                request.NeedsGPS,
                request.NeedsChildSeat
            );

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }
    }
}
