using CarRentalSystem.DAL.Data;
using CarRentalSystem.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.Utils
{
    public class SeedData : ISeedData
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public SeedData(
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task DataSeedingAsync()
        {
            if ((await _context.Database.GetPendingMigrationsAsync()).Any())
            {
                await _context.Database.MigrateAsync();
            }

            if (!await _context.Cars.AnyAsync())
            {
                await _context.Cars.AddRangeAsync(
                    new Car
                    {
                        Make = "Toyota",
                        Model = "Yaris",
                        Year = 2023,
                        Color = "White",
                        PlateNumber = "ABC-1001",
                        Type = CarType.Economy,
                        Transmission = TransmissionType.Automatic,
                        SeatingCapacity = 5,
                        FuelType = FuelType.Petrol,
                        Mileage = 15000,
                        DailyRate = 35.00m,
                        WeeklyRate = 200.00m,
                        MonthlyRate = 700.00m,
                        Status = CarStatus.Available,
                        Description = "Fuel-efficient economy car, perfect for city driving",
                        HasAirConditioning = true,
                        HasGPS = false,
                        HasBluetooth = true,
                        HasBackupCamera = true,
                        HasSunroof = false
                    },
                    new Car
                    {
                        Make = "Hyundai",
                        Model = "Accent",
                        Year = 2023,
                        Color = "White",
                        PlateNumber = "ABC-1002",
                        Type = CarType.Economy,
                        Transmission = TransmissionType.Automatic,
                        SeatingCapacity = 5,
                        FuelType = FuelType.Petrol,
                        Mileage = 12000,
                        DailyRate = 30.00m,
                        WeeklyRate = 180.00m,
                        MonthlyRate = 650.00m,
                        Status = CarStatus.Available,
                        Description = "Comfortable and affordable economy car",
                        HasAirConditioning = true,
                        HasGPS = false,
                        HasBluetooth = true,
                        HasBackupCamera = false,
                        HasSunroof = false
                    },

                    new Car
                    {
                        Make = "Toyota",
                        Model = "Camry",
                        Year = 2024,
                        Color = "Black",
                        PlateNumber = "ABC-2001",
                        Type = CarType.Sedan,
                        Transmission = TransmissionType.Automatic,
                        SeatingCapacity = 5,
                        FuelType = FuelType.Hybrid,
                        Mileage = 8000,
                        DailyRate = 55.00m,
                        WeeklyRate = 350.00m,
                        MonthlyRate = 1200.00m,
                        Status = CarStatus.Available,
                        Description = "Premium sedan with hybrid technology",
                        HasAirConditioning = true,
                        HasGPS = true,
                        HasBluetooth = true,
                        HasBackupCamera = true,
                        HasSunroof = true
                    },
                    new Car
                    {
                        Make = "Honda",
                        Model = "Accord",
                        Year = 2023,
                        Color = "Blue",
                        PlateNumber = "ABC-2002",
                        Type = CarType.Sedan,
                        Transmission = TransmissionType.Automatic,
                        SeatingCapacity = 5,
                        FuelType = FuelType.Petrol,
                        Mileage = 18000,
                        DailyRate = 50.00m,
                        WeeklyRate = 320.00m,
                        MonthlyRate = 1100.00m,
                        Status = CarStatus.Available,
                        Description = "Reliable and spacious family sedan",
                        HasAirConditioning = true,
                        HasGPS = true,
                        HasBluetooth = true,
                        HasBackupCamera = true,
                        HasSunroof = false
                    },

                    
                    new Car
                    {
                        Make = "Toyota",
                        Model = "RAV4",
                        Year = 2024,
                        Color = "Red",
                        PlateNumber = "ABC-3001",
                        Type = CarType.SUV,
                        Transmission = TransmissionType.Automatic,
                        SeatingCapacity = 7,
                        FuelType = FuelType.Hybrid,
                        Mileage = 5000,
                        DailyRate = 75.00m,
                        WeeklyRate = 480.00m,
                        MonthlyRate = 1700.00m,
                        Status = CarStatus.Available,
                        Description = "Spacious SUV perfect for family trips",
                        HasAirConditioning = true,
                        HasGPS = true,
                        HasBluetooth = true,
                        HasBackupCamera = true,
                        HasSunroof = true
                    },
                    new Car
                    {
                        Make = "Nissan",
                        Model = "X-Trail",
                        Year = 2023,
                        Color = "Gray",
                        PlateNumber = "ABC-3002",
                        Type = CarType.SUV,
                        Transmission = TransmissionType.Automatic,
                        SeatingCapacity = 7,
                        FuelType = FuelType.Petrol,
                        Mileage = 22000,
                        DailyRate = 65.00m,
                        WeeklyRate = 420.00m,
                        MonthlyRate = 1500.00m,
                        Status = CarStatus.Available,
                        Description = "Versatile SUV with plenty of cargo space",
                        HasAirConditioning = true,
                        HasGPS = true,
                        HasBluetooth = true,
                        HasBackupCamera = true,
                        HasSunroof = false
                    },

                    new Car
                    {
                        Make = "BMW",
                        Model = "5 Series",
                        Year = 2024,
                        Color = "Black",
                        PlateNumber = "ABC-4001",
                        Type = CarType.Luxury,
                        Transmission = TransmissionType.Automatic,
                        SeatingCapacity = 5,
                        FuelType = FuelType.Hybrid,
                        Mileage = 3000,
                        DailyRate = 120.00m,
                        WeeklyRate = 750.00m,
                        MonthlyRate = 2800.00m,
                        Status = CarStatus.Available,
                        Description = "Luxury sedan with premium features",
                        HasAirConditioning = true,
                        HasGPS = true,
                        HasBluetooth = true,
                        HasBackupCamera = true,
                        HasSunroof = true
                    },
                    new Car
                    {
                        Make = "Mercedes-Benz",
                        Model = "E-Class",
                        Year = 2024,
                        Color = "Silver",
                        PlateNumber = "ABC-4002",
                        Type = CarType.Luxury,
                        Transmission = TransmissionType.Automatic,
                        SeatingCapacity = 5,
                        FuelType = FuelType.Hybrid,
                        Mileage = 2000,
                        DailyRate = 150.00m,
                        WeeklyRate = 950.00m,
                        MonthlyRate = 3500.00m,
                        Status = CarStatus.Available,
                        Description = "Executive luxury sedan with cutting-edge technology",
                        HasAirConditioning = true,
                        HasGPS = true,
                        HasBluetooth = true,
                        HasBackupCamera = true,
                        HasSunroof = true
                    },

                    new Car
                    {
                        Make = "Ford",
                        Model = "Mustang",
                        Year = 2024,
                        Color = "Yellow",
                        PlateNumber = "ABC-5001",
                        Type = CarType.Sport,
                        Transmission = TransmissionType.Automatic,
                        SeatingCapacity = 4,
                        FuelType = FuelType.Petrol,
                        Mileage = 1000,
                        DailyRate = 180.00m,
                        WeeklyRate = 1150.00m,
                        MonthlyRate = 4200.00m,
                        Status = CarStatus.Available,
                        Description = "Iconic sports car with powerful performance",
                        HasAirConditioning = true,
                        HasGPS = true,
                        HasBluetooth = true,
                        HasBackupCamera = true,
                        HasSunroof = false
                    },

                    new Car
                    {
                        Make = "Toyota",
                        Model = "Hiace",
                        Year = 2023,
                        Color = "White",
                        PlateNumber = "ABC-6001",
                        Type = CarType.Van,
                        Transmission = TransmissionType.Manual,
                        SeatingCapacity = 12,
                        FuelType = FuelType.Diesel,
                        Mileage = 35000,
                        DailyRate = 80.00m,
                        WeeklyRate = 500.00m,
                        MonthlyRate = 1800.00m,
                        Status = CarStatus.Available,
                        Description = "Spacious van for group transportation",
                        HasAirConditioning = true,
                        HasGPS = false,
                        HasBluetooth = true,
                        HasBackupCamera = true,
                        HasSunroof = false
                    }
                );

                await _context.SaveChangesAsync();
            }

            // Seed Car Images
            if (!await _context.CarImages.AnyAsync())
            {
                var cars = await _context.Cars.ToListAsync();
                var carImages = new List<CarImage>();

                foreach (var car in cars)
                {
                    carImages.Add(new CarImage
                    {
                        CarId = car.Id,
                        ImageUrl = $"/images/default/{car.Make.ToLower()}-{car.Model.ToLower()}-1.jpg",
                        IsMain = true
                    });

                    carImages.Add(new CarImage
                    {
                        CarId = car.Id,
                        ImageUrl = $"/images/default/{car.Make.ToLower()}-{car.Model.ToLower()}-2.jpg",
                        IsMain = false
                    });

                    carImages.Add(new CarImage
                    {
                        CarId = car.Id,
                        ImageUrl = $"/images/default/{car.Make.ToLower()}-{car.Model.ToLower()}-3.jpg",
                        IsMain = false
                    });
                }

                await _context.CarImages.AddRangeAsync(carImages);
                await _context.SaveChangesAsync();
            }
        }

        public async Task IdentityDataSeedingAsync()
        {
            if (!await _roleManager.Roles.AnyAsync())
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
                await _roleManager.CreateAsync(new IdentityRole("Customer"));
                await _roleManager.CreateAsync(new IdentityRole("Staff"));
            }

            if (!await _userManager.Users.AnyAsync())
            {
                // Admin User
                var admin = new ApplicationUser
                {
                    Email = "ahmadheeba10@gmail.com",
                    FullName = "Ahmad Baker",
                    PhoneNumber = "0568178976",
                    UserName = "ahmad_baker",
                    EmailConfirmed = true,
                    Address = "123 Admin Street",
                    City = "Dubai",
                    DateOfBirth = new DateTime(2005, 1, 2)
                };
                var result1 = await _userManager.CreateAsync(admin, "Admin@123");
                if (result1.Succeeded)
                    await _userManager.AddToRoleAsync(admin, "Admin");
                else
                    throw new Exception(string.Join(", ", result1.Errors.Select(e => e.Description)));

                // Staff User
                var staff = new ApplicationUser
                {
                    Email = "staff@carrental.com",
                    FullName = "Ahmed Hassan",
                    PhoneNumber = "0568125658",
                    UserName = "staff_ahmed",
                    EmailConfirmed = true,
                    Address = "456 Staff Avenue",
                    City = "Abu Dhabi",
                    DateOfBirth = new DateTime(1990, 5, 20)
                };
                var result2 = await _userManager.CreateAsync(staff, "Staff@123");
                if (result2.Succeeded)
                    await _userManager.AddToRoleAsync(staff, "Staff");
                else
                    throw new Exception(string.Join(", ", result2.Errors.Select(e => e.Description)));

                // Customer Users
                var customer1 = new ApplicationUser
                {
                    Email = "john@example.com",
                    FullName = "John Smith",
                    PhoneNumber = "0568456852",
                    UserName = "john_smith",
                    EmailConfirmed = true,
                    Address = "789 Customer Road",
                    City = "Sharjah",
                    DriverLicenseNumber = "DL12345678",
                    DriverLicenseExpiry = DateTime.UtcNow.AddYears(3),
                    DateOfBirth = new DateTime(1992, 8, 10)
                };
                var result3 = await _userManager.CreateAsync(customer1, "Customer@123");
                if (result3.Succeeded)
                    await _userManager.AddToRoleAsync(customer1, "Customer");
                else
                    throw new Exception(string.Join(", ", result3.Errors.Select(e => e.Description)));

                var customer2 = new ApplicationUser
                {
                    Email = "sarah@example.com",
                    FullName = "Sarah Mohammed",
                    PhoneNumber = "0568789456",
                    UserName = "sarah_m",
                    EmailConfirmed = true,
                    Address = "321 Client Street",
                    City = "Ajman",
                    DriverLicenseNumber = "DL87654321",
                    DriverLicenseExpiry = DateTime.UtcNow.AddYears(5),
                    DateOfBirth = new DateTime(1995, 3, 25)
                };
                var result4 = await _userManager.CreateAsync(customer2, "Customer@123");
                if (result4.Succeeded)
                    await _userManager.AddToRoleAsync(customer2, "Customer");
                else
                    throw new Exception(string.Join(", ", result4.Errors.Select(e => e.Description)));
            }

            await _context.SaveChangesAsync();
        }
    }
}
