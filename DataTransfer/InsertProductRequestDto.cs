using System.ComponentModel.DataAnnotations;

namespace GlobalApi.DataTransfer
{
    public class InsertProductRequestDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string City { get; set; } = string.Empty;
        [Required]
        public string State { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
        [Required]
        public int AvailableUnits { get; set; } = 0;
        public bool Wifi { get; set; } = false;
        public bool Laundry { get; set; } = false;

    }
}
