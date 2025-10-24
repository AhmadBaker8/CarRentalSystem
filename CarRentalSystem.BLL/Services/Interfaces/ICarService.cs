using CarRentalSystem.BLL.Common;
using CarRentalSystem.DAL.DTO.Requests;
using CarRentalSystem.DAL.DTO.Responses;
using CarRentalSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.BLL.Services.Interfaces
{
    public interface ICarService
    {
        // Customer
        Task<ServiceResult<List<CarResponse>>> GetAllCarsAsync(HttpRequest httpRequest = null);
        Task<ServiceResult<CarDetailResponse>> GetCarDetailsAsync(int carId, HttpRequest httpRequest = null);
        Task<ServiceResult<List<CarResponse>>> SearchCarsAsync(string searchTerm, HttpRequest httpRequest = null);
        Task<ServiceResult<List<CarResponse>>> FilterCarsAsync(CarFilterRequest request, HttpRequest httpRequest = null);

        // Admin
        Task<ServiceResult<CarResponse>> AddCarAsync(AddCarRequest request, HttpRequest httpRequest = null);
        Task<ServiceResult<CarResponse>> UpdateCarAsync(UpdateCarRequest request, HttpRequest httpRequest = null);
        Task<ServiceResult<bool>> DeleteCarAsync(int carId);
        Task<ServiceResult<List<CarResponse>>> GetAllCarsAdminAsync(HttpRequest httpRequest = null);
    }
}
