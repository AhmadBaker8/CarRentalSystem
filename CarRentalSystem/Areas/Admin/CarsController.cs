using CarRentalSystem.BLL.Services.Interfaces;
using CarRentalSystem.DAL.DTO.Requests;
using CarRentalSystem.DAL.Models;
using CarRentalSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Areas.Admin
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    public class CarsController : ControllerBase
    {
        private readonly ICarService _carService;
        private readonly IFileService _fileService;
        private readonly ICarImageRepository _carImageRepository;
        public CarsController(ICarService carService, IFileService fileService, ICarImageRepository carImageRepository)
        {
            _carService = carService;
            _fileService = fileService;
            _carImageRepository = carImageRepository;
        }


        // GET: api/Admin/Cars
        [HttpGet]
        public async Task<IActionResult> GetAllCars()
        {
            var result = await _carService.GetAllCarsAdminAsync(Request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // POST: api/Admin/Cars
        [HttpPost]
        public async Task<IActionResult> AddCar([FromForm] AddCarRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // إضافة السيارة
                var carResult = await _carService.AddCarAsync(request,Request);

                if (!carResult.Success)
                {
                    return BadRequest(carResult);
                }
             
                return Ok(new { message = "Car added successfully", data = carResult.Data });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred", error = ex.Message });
            }
        }
       
        // PUT: api/Admin/Cars/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCar(int id, [FromBody] UpdateCarRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != request.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            var result = await _carService.UpdateCarAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // DELETE: api/Admin/Cars/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var result = await _carService.DeleteCarAsync(id);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
