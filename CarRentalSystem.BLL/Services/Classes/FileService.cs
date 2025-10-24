using CarRentalSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.BLL.Services.Classes
{
    public class FileService : IFileService
    {
        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
                using (var stream = File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }
                return fileName;
            }
            throw new Exception("File is empty or null");
        }

        public async Task<List<string>> UploadManyAsync(List<IFormFile> files)
        {
            var fileNames = new List<string>();
            foreach (var file in files)
            {
                if (file != null && file.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
                    using (var stream = File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                    }
                    fileNames.Add(fileName);

                }
            }
            return fileNames;
        }
    }
}
