using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.DAL.DTO.Responses
{
    public class UserResponse
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpiration { get; set; }
    }
}
