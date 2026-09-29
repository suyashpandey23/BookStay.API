using System.ComponentModel.DataAnnotations;

namespace BookStay.API.DTOs.Country;

public class UpdateCountryDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(3)]
    [MinLength(2)]
    public string Code { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Continent { get; set; } = string.Empty;
}
