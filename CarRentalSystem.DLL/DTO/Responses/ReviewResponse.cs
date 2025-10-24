using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.DTO.Responses
{
    public class ReviewResponse
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public int BookingId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CarReviewsResponse
    {
        public int CarId { get; set; }
        public string CarName { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public List<ReviewResponse> Reviews { get; set; } = new();
    }
}
