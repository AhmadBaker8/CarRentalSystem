using CarRentalSystem.DAL.Data;
using CarRentalSystem.DAL.Models;
using CarRentalSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.Repositories.Classes
{
    public class CarImageRepository : ICarImageRepository
    {
        private readonly ApplicationDbContext _context;
        public CarImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<CarImage> AddAsync(CarImage carImage)
        {
            await _context.CarImages.AddAsync(carImage);
            await _context.SaveChangesAsync();
            return carImage;
        }

        public async Task<int> UpdateAsync(CarImage carImage)
        {
            _context.CarImages.Update(carImage);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var carImage = await _context.CarImages.FindAsync(id);
            if (carImage == null) return false;

            carImage.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<CarImage>> GetCarImagesAsync(int carId)
        {
            return await _context.CarImages
                .Where(ci => ci.CarId == carId && !ci.IsDeleted)
                .ToListAsync();
        }
    }
}
