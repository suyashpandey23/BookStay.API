using System.ComponentModel.DataAnnotations;

namespace BookStay.API.DTOs.Stay;

public class UpdateStayDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;
    
    [Range(0, 5)]
    public double Rating { get; set; }
    
    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;
    
    [Required]
    public int CountryId { get; set; }
}
