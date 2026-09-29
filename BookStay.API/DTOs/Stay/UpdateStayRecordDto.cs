namespace BookStay.API.DTOs.Stay;

public record UpdateStayRecordDto(string Name, string Address, string City, double Rating, string ImageUrl, int CountryId);
