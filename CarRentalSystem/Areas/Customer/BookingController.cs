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
        public async Task<IActionResult> CheckAvailability([FromBody] dynamic request)
        {
            int carId = request.carId;
            DateTime pickupDate = request.pickupDate;
            DateTime returnDate = request.returnDate;

            var result = await _bookingService.IsCarAvailableAsync(carId, pickupDate, returnDate);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        // POST: api/Customer/Bookings/calculate-price
        [HttpPost("calculate-price")]
        [AllowAnonymous]
        public async Task<IActionResult> CalculatePrice([FromBody] dynamic request)
        {
            int carId = request.carId;
            DateTime pickupDate = request.pickupDate;
            DateTime returnDate = request.returnDate;
            bool hasInsurance = request.hasInsurance ?? false;
            bool needsGPS = request.needsGPS ?? false;
            bool needsChildSeat = request.needsChildSeat ?? false;

            var result = await _bookingService.CalculateBookingPriceAsync(
                carId, pickupDate, returnDate, hasInsurance, needsGPS, needsChildSeat
            );

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }
    }
}
