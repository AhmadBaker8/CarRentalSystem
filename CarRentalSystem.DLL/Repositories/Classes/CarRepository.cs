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
    public class CarRepository : ICarRepository
    {
        private readonly ApplicationDbContext _context;
        public CarRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Car> AddAsync(Car car)
        {
            await _context.Cars.AddAsync(car);
            await _context.SaveChangesAsync();
            return car;
        }

        public async Task<int> UpdateAsync(Car car)
        {
            _context.Cars.Update(car);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return false;

            car.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Car> GetByIdAsync(int id)
        {
            return await _context.Cars.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Car> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Cars
                .Include(c => c.CarImages)
                .Include(c => c.Reviews)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Car>> GetAllAsync()
        {
            return await _context.Cars
                .Include(c => c.CarImages)
                .Where(c => c.Status == CarStatus.Available)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Car>> GetAvailableCarsAsync()
        {
            return await _context.Cars
                .Include(c => c.CarImages)
                .Where(c => c.Status == CarStatus.Available)
                .OrderBy(c => c.Make)
                .ToListAsync();
        }

        public async Task<List<Car>> SearchAsync(string searchTerm)
        {
            return await _context.Cars
                .Include(c => c.CarImages)
                .Where(c =>
                    (c.Make.ToLower().Contains(searchTerm.ToLower()) ||
                     c.Model.ToLower().Contains(searchTerm.ToLower()) ||
                     c.PlateNumber.ToLower().Contains(searchTerm.ToLower())) &&
                    c.Status == CarStatus.Available)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Car>> FilterAsync(
            CarType? type,
            TransmissionType? transmission,
            FuelType? fuelType,
            decimal? minPrice,
            decimal? maxPrice,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.Cars
                .Include(c => c.CarImages)
                .Where(c => c.Status == CarStatus.Available);

            if (type.HasValue)
                query = query.Where(c => c.Type == type);

            if (transmission.HasValue)
                query = query.Where(c => c.Transmission == transmission);

            if (fuelType.HasValue)
                query = query.Where(c => c.FuelType == fuelType);

            if (minPrice.HasValue)
                query = query.Where(c => c.DailyRate >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(c => c.DailyRate <= maxPrice);

            return await query
                .OrderBy(c => c.DailyRate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<bool> IsPlateNumberUniqueAsync(string plateNumber, int? excludeCarId = null)
        {
            var query = _context.Cars.Where(c => c.PlateNumber == plateNumber);

            if (excludeCarId.HasValue)
                query = query.Where(c => c.Id != excludeCarId);

            return !await query.AnyAsync();
        }
    }
}
