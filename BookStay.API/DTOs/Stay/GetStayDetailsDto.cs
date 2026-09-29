namespace BookStay.API.DTOs.Stay;

public record GetStayDetailsDto(int Id, string Name, string Address, string City, double Rating, string ImageUrl, int CountryId, CountryDto? Country);
