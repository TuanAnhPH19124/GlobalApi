using GlobalApi.Models;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson.IO;
using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace GlobalApi.DataTransfer
{
    public class UpdateProductRequestDto
    {
        [Required]
        public string Housing { get; set; } = string.Empty;
        public IFormFile? Images { get; set; }
    }
}
