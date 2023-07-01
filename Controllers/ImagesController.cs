using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GlobalApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ImagesController : Controller
  {

    public ImagesController()
    {

    }

    [HttpGet]
    [Route("api/images")]
    public IActionResult GetImages()
    {
      var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload");
      var imageFiles = Directory.GetFiles(uploadFolder);
      var imageUrls = imageFiles.Select(file =>
      {
        var imageName = Path.GetFileName(file);
        return Url.Link("GetImage", new { imageName = imageName });
      });

      return Ok(imageUrls);
    }



  }
}