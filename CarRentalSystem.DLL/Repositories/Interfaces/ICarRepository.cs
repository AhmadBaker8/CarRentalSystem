using CarRentalSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.Repositories.Interfaces
{
    public interface ICarRepository
    {
        Task<Car> AddAsync(Car car);
        Task<int> UpdateAsync(Car car);
        Task<bool> DeleteAsync(int id);
        Task<Car> GetByIdAsync(int id);
        Task<Car> GetByIdWithDetailsAsync(int id);
        Task<List<Car>> GetAllAsync();
        Task<List<Car>> GetAvailableCarsAsync();
        Task<List<Car>> SearchAsync(string searchTerm);
        Task<List<Car>> FilterAsync(
            CarType? type,
            TransmissionType? transmission,
            FuelType? fuelType,
            decimal? minPrice,
            decimal? maxPrice,
            int pageNumber = 1,
            int pageSize = 10);
        Task<bool> IsPlateNumberUniqueAsync(string plateNumber, int? excludeCarId = null);
    }
}
