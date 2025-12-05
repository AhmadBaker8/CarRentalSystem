using CarRentalSystem.DAL.Models;
using CarRentalSystem.DAL.DTO.Responses;
using Mapster;
using System.Linq;

namespace CarRentalSystem.BLL
{
    public static class MapsterConfig
    {
        public static void RegisterMappings()
        {
            // 1) CarImage -> CarImageResponse
            TypeAdapterConfig<CarImage, CarImageResponse>
                .NewConfig();

            // 2) Review -> ReviewResponse
            TypeAdapterConfig<Review, ReviewResponse>
                .NewConfig()
                .Map(dest => dest.UserName, src => src.User != null ? src.User.UserName : "Anonymous")
                .Map(dest => dest.UserFullName, src => src.User != null ? src.User.FullName : "Anonymous");

            // 3) Car -> CarResponse
            TypeAdapterConfig<Car, CarResponse>
                .NewConfig()
                // ربط CarImages مع Images
                .Map(dest => dest.Images, src => src.CarImages)
                // حساب AverageRating و ReviewCount
                .Map(dest => dest.AverageRating,
                     src => src.Reviews != null && src.Reviews.Any()
                            ? src.Reviews.Average(r => r.Rating)
                            : 0)
                .Map(dest => dest.ReviewCount,
                     src => src.Reviews != null ? src.Reviews.Count : 0);

            // 4) Car -> CarDetailResponse (توريث إعدادات CarResponse + إضافة Reviews)
            TypeAdapterConfig<Car, CarDetailResponse>
                .NewConfig()
                .Inherits<Car, CarResponse>() // ياخذ كل المابينغ من CarResponse
                .Map(dest => dest.Reviews, src => src.Reviews);


            // Booking -> BookingResponse
            TypeAdapterConfig<Booking, BookingResponse>
                .NewConfig()
                .Map(dest => dest.CarMakeModel,
                     src => src.Car != null ? $"{src.Car.Make} {src.Car.Model}" : null)
                .Map(dest => dest.CarImage,
                     src => src.Car != null
                         ? src.Car.CarImages
                             .Where(i => i.IsMain)
                             .Select(i => i.ImageUrl)
                             .FirstOrDefault()
                         : null)
                .Map(dest => dest.UserName,
                     src => src.User != null ? src.User.UserName : null);

            // Booking -> BookingSummaryResponse
            TypeAdapterConfig<Booking, BookingSummaryResponse>
                .NewConfig()
                .Map(dest => dest.CarMakeModel,
                     src => src.Car != null ? $"{src.Car.Make} {src.Car.Model}" : null);


            TypeAdapterConfig<Review, ReviewResponse>
                .NewConfig()
                .Map(dest => dest.CarId, src => src.CarId)
                .Map(dest => dest.BookingId, src => src.BookingId)
                .Map(dest => dest.UserName,
                     src => src.User != null ? src.User.UserName : "Anonymous")
                .Map(dest => dest.UserFullName,
                     src => src.User != null ? src.User.FullName : "Anonymous");
        }
    }
}