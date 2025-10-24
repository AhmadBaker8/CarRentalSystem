using CarRentalSystem.BLL.Services.Interfaces;
using CarRentalSystem.DAL.DTO.Requests;
using CarRentalSystem.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Areas.Admin
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }
        // GET: api/Admin/Bookings
        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var result = await _bookingService.GetAllBookingsAsync(Request);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        // GET: api/Admin/Bookings/status/Pending
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetBookingsByStatus(BookingStatus status)
        {
            var result = await _bookingService.GetBookingsByStatusAsync(status, Request);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        // PUT: api/Admin/Bookings/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] UpdateBookingStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _bookingService.UpdateBookingStatusAsync(id, request);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        // POST: api/Admin/Bookings/5/pickup
        [HttpPost("{id}/pickup")]
        public async Task<IActionResult> MarkAsPickedUp(int id, [FromBody] MarkAsPickedUpRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _bookingService.MarkAsPickedUpAsync(id, request.PickupMileage);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        // POST: api/Admin/Bookings/5/return
        [HttpPost("{id}/return")]
        public async Task<IActionResult> ProcessReturn(int id, [FromBody] ProcessReturnRequest request)
        {
            if (!ModelState.IsValid)
                { return BadRequest(ModelState); }


            var result = await _bookingService.ProcessReturnAsync(id, request.ReturnMileage, request.ReturnConditionNotes);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }
    }
}
