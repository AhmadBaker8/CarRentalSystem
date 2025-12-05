using CarRentalSystem.BLL.Common;
using CarRentalSystem.BLL.Services.Interfaces;
using CarRentalSystem.DAL.DTO.Requests;
using CarRentalSystem.DAL.DTO.Responses;
using CarRentalSystem.DAL.Models;
using CarRentalSystem.DAL.Repositories.Classes;
using CarRentalSystem.DAL.Repositories.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Http;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.BLL.Services.Classes
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepository;
        private readonly IFileService _fileService;
        private readonly ICarImageRepository _carImageRepository;
        public CarService(ICarRepository carRepository, IFileService fileService, ICarImageRepository carImageRepository)
        {
            _carRepository = carRepository;
            _fileService = fileService;
            _carImageRepository = carImageRepository;
        }


        // Customer Methods
        public async Task<ServiceResult<List<CarResponse>>> GetAllCarsAsync(HttpRequest httpRequest = null)
        {
            try
            {
                var cars = await _carRepository.GetAvailableCarsAsync();
                var response = cars.Adapt<List<CarResponse>>();

                SetFullImageUrls(response,httpRequest);

                return ServiceResult<List<CarResponse>>.SuccessResult(
                    response,
                    "Cars retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<List<CarResponse>>.FailureResult(
                    "Failed to retrieve cars",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<CarDetailResponse>> GetCarDetailsAsync(int carId, HttpRequest httpRequest = null)
        {
            try
            {
                var car = await _carRepository.GetByIdWithDetailsAsync(carId);

                if (car == null)
                {
                    return ServiceResult<CarDetailResponse>.FailureResult(
                        "Car not found",
                        new List<string> { "Car not found" }
                    );
                }

                var response = car.Adapt<CarDetailResponse>();

                return ServiceResult<CarDetailResponse>.SuccessResult(
                    response,
                    "Car details retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<CarDetailResponse>.FailureResult(
                    "Failed to retrieve car details",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<List<CarResponse>>> SearchCarsAsync(string searchTerm, HttpRequest httpRequest = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return ServiceResult<List<CarResponse>>.FailureResult(
                        "Search term is required",
                        new List<string> { "Please enter a search term" }
                    );
                }

                var cars = await _carRepository.SearchAsync(searchTerm);
                var response = cars.Adapt<List<CarResponse>>();

                return ServiceResult<List<CarResponse>>.SuccessResult(
                    response,
                    $"Found {response.Count} car(s)"
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<List<CarResponse>>.FailureResult(
                    "Failed to search cars",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<List<CarResponse>>> FilterCarsAsync(CarFilterRequest request, HttpRequest httpRequest = null)
        {
            try
            {
                var cars = await _carRepository.FilterAsync(
                    request.Type,
                    request.Transmission,
                    request.FuelType,
                    request.MinPrice,
                    request.MaxPrice,
                    request.PageNumber,
                    request.PageSize
                );

                var response = cars.Adapt<List<CarResponse>>();

                return ServiceResult<List<CarResponse>>.SuccessResult(
                    response,
                    "Cars filtered successfully"
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<List<CarResponse>>.FailureResult(
                    "Failed to filter cars",
                    new List<string> { ex.Message }
                );
            }
        }

        // Admin Methods
        public async Task<ServiceResult<CarResponse>> AddCarAsync(AddCarRequest request, HttpRequest httpRequest = null)
        {
            try
            {
                var isUnique = await _carRepository.IsPlateNumberUniqueAsync(request.PlateNumber);
                if (!isUnique)
                {
                    return ServiceResult<CarResponse>.FailureResult(
                        "Add car failed",
                        new List<string> { "Plate number already exists" }
                    );
                }
                var car = request.Adapt<Car>();
                

                var addedCar = await _carRepository.AddAsync(car);

                if (request.MainImage != null)
                {
                    var mainImagePath = await _fileService.UploadImageAsync(request.MainImage);
                    await _carImageRepository.AddAsync(new CarImage
                    {
                        CarId = addedCar.Id,
                        ImageUrl = mainImagePath,
                        IsMain = true
                    });
                }

                // رفع الصور الإضافية
                if (request.SubImages != null && request.SubImages.Count > 0)
                {
                    var subImagePaths = await _fileService.UploadManyAsync(request.SubImages);
                    foreach (var imagePath in subImagePaths)
                    {
                        await _carImageRepository.AddAsync(new CarImage
                        {
                            CarId = addedCar.Id,
                            ImageUrl = imagePath,
                            IsMain = false
                        });
                    }
                }
                var fullCar = await _carRepository.GetByIdWithDetailsAsync(addedCar.Id);
                var response = fullCar.Adapt<CarResponse>();
                SetFullImageUrls(response,httpRequest);

                return ServiceResult<CarResponse>.SuccessResult(
                    response,
                    "Car added successfully"
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<CarResponse>.FailureResult(
                    "Failed to add car",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<CarResponse>> UpdateCarAsync(UpdateCarRequest request, HttpRequest httpRequest = null)
        {
            try
            {
                var car = await _carRepository.GetByIdAsync(request.Id);

                if (car == null)
                {
                    return ServiceResult<CarResponse>.FailureResult(
                        "Car not found",
                        new List<string> { "Car not found" }
                    );
                }

                // التحقق من فريدية رقم اللوحة
                if (car.PlateNumber != request.PlateNumber)
                {
                    var isUnique = await _carRepository.IsPlateNumberUniqueAsync(request.PlateNumber, request.Id);
                    if (!isUnique)
                    {
                        return ServiceResult<CarResponse>.FailureResult(
                            "Update failed",
                            new List<string> { "Plate number already exists" }
                        );
                    }
                }

                car.Make = request.Make;
                car.Model = request.Model;
                car.Year = request.Year;
                car.Color = request.Color;
                car.PlateNumber = request.PlateNumber;
                car.Type = request.Type;
                car.Transmission = request.Transmission;
                car.SeatingCapacity = request.SeatingCapacity;
                car.FuelType = request.FuelType;
                car.Mileage = request.Mileage;
                car.DailyRate = request.DailyRate;
                car.WeeklyRate = request.WeeklyRate;
                car.MonthlyRate = request.MonthlyRate;
                car.Description = request.Description;
                car.Status = request.Status;
                car.HasAirConditioning = request.HasAirConditioning;
                car.HasGPS = request.HasGPS;
                car.HasBluetooth = request.HasBluetooth;
                car.HasBackupCamera = request.HasBackupCamera;
                car.HasSunroof = request.HasSunroof;
                car.UpdatedAt = DateTime.UtcNow;

                await _carRepository.UpdateAsync(car);
                var response = car.Adapt<CarResponse>();
                SetFullImageUrls(response,httpRequest);

                return ServiceResult<CarResponse>.SuccessResult(
                    response,
                    "Car updated successfully"
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<CarResponse>.FailureResult(
                    "Failed to update car",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<bool>> DeleteCarAsync(int carId)
        {
            try
            {
                var result = await _carRepository.DeleteAsync(carId);

                if (!result)
                {
                    return ServiceResult<bool>.FailureResult(
                        "Car not found",
                        new List<string> { "Car not found" }
                    );
                }

                return ServiceResult<bool>.SuccessResult(true, "Car deleted successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.FailureResult(
                    "Failed to delete car",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ServiceResult<List<CarResponse>>> GetAllCarsAdminAsync(HttpRequest httpRequest = null)
        {
            try
            {
                var cars = await _carRepository.GetAllAsync();
                var response = cars.Adapt<List<CarResponse>>();

                return ServiceResult<List<CarResponse>>.SuccessResult(
                    response,
                    "Cars retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<List<CarResponse>>.FailureResult(
                    "Failed to retrieve cars",
                    new List<string> { ex.Message }
                );
            }
        }

        // Helper Methods
        private static string BuildImageUrl(string imagePath, HttpRequest httpRequest)
        {
            if (string.IsNullOrEmpty(imagePath))
                return string.Empty;

            if (httpRequest == null)
                return imagePath; // رجّع الـ path كما هو (relative) لو مافي request

            return $"{httpRequest.Scheme}://{httpRequest.Host}/images/{imagePath}";
        }

        private static void SetFullImageUrls(IEnumerable<CarResponse> cars, HttpRequest httpRequest)
        {
            if (httpRequest == null || cars == null)
                return;

            foreach (var car in cars)
            {
                if (car.Images == null) continue;

                foreach (var img in car.Images)
                {
                    img.ImageUrl = BuildImageUrl(img.ImageUrl, httpRequest);
                }
            }
        }

        private static void SetFullImageUrls(CarResponse car, HttpRequest httpRequest)
        {
            if (httpRequest == null || car == null || car.Images == null)
                return;

            foreach (var img in car.Images)
            {
                img.ImageUrl = BuildImageUrl(img.ImageUrl, httpRequest);
            }
        }

        private static void SetFullImageUrls(CarDetailResponse car, HttpRequest httpRequest)
        {
            if (httpRequest == null || car == null || car.Images == null)
                return;

            foreach (var img in car.Images)
            {
                img.ImageUrl = BuildImageUrl(img.ImageUrl, httpRequest);
            }
        }
    }

}
