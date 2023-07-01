using GlobalApi.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlobalApi.Models;

[Table("products")]
public class Product
{
    
    public string Id { get; set; } = Guid.NewGuid().ToString();


    public string Name { get; set; } = string.Empty;


    public string City { get; set; } = string.Empty;


    public string State { get; set; } = string.Empty;


    public string Photo { get; set; } = string.Empty;
 
    public int AvailableUnits { get; set; }

    public bool Wifi { get; set; }

    public bool Laundry { get; set; }
   

}
