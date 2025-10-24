using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.DTO.Requests
{
    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }


        [Required(ErrorMessage = "Code is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Code must be 6 characters")]
        public string Code { get; set; }


        [Required(ErrorMessage = "New password is required")]
        [MinLength(10, ErrorMessage = "Password must be at least 10 characters")]
        public string NewPassword { get; set; }
    }
}
