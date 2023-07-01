using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlobalApi.Ultilities
{
  public static class UploadService
  {
    public static void UploadImages(IFormFile Image, string id)
    {
      try
      {
        var extension = Path.GetExtension(Image.FileName);
        var fileName =  Guid.NewGuid().ToString() + extension;
        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", id);
        var filePath = Path.Combine(uploadPath, fileName);
        if (Image.Length > 0)
        {
          if (!Directory.Exists(uploadPath))
          {
            Directory.CreateDirectory(uploadPath);
          }
          using (var fileStream = System.IO.File.Create(filePath))
          {
            Image.CopyTo(fileStream);
            fileStream.Flush();
          };
        }
      }
      catch (System.Exception)
      {
        throw;
      }
    }
  }
}