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
    //[Authorize(Roles ="Customer")]
    public class CarsController : ControllerBase
    {
        private readonly ICarService _carService;
        public CarsController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCars()
        {
            var result = await _carService.GetAllCarsAsync(Request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarDetails(int id)
        {
            var result = await _carService.GetCarDetailsAsync(id);

            if (result.Success)
            {
                return Ok(result);
            }

            return result.Message == "Car not found" ? NotFound(result) : BadRequest(result);
        }

        // GET: api/Customer/Cars/search?term=toyota
        [HttpGet("search")]
        public async Task<IActionResult> SearchCars([FromQuery] string term)
        {
            var result = await _carService.SearchCarsAsync(term);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // POST: api/Customer/Cars/filter
        [HttpPost("filter")]
        public async Task<IActionResult> FilterCars([FromBody] CarFilterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _carService.FilterCarsAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
