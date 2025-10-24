using CarRentalSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Areas.Admin
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReviews()
        {
            var result = await _reviewService.GetAllReviewsAsync();
            if (result.Success)
                return Ok(result);

            return BadRequest(result);

        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var result = await _reviewService.DeleteReviewAdminAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
