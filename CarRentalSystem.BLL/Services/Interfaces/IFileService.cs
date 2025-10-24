using CarRentalSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.BLL.Services.Interfaces
{
    public interface IFileService
    {

        Task<string> UploadImageAsync(IFormFile file);

        Task<List<string>> UploadManyAsync(List<IFormFile> files);
    }
}
