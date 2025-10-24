using CarRentalSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.Repositories.Interfaces
{
    public interface ICarImageRepository
    {
        Task<CarImage> AddAsync(CarImage carImage);
        Task<int> UpdateAsync(CarImage carImage);
        Task<bool> DeleteAsync(int id);
        Task<List<CarImage>> GetCarImagesAsync(int carId);
    }
}
