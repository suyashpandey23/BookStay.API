namespace BookStay.API.DTOs.Country;

public record GetCountryDetailsDto(int Id, string Name, string Code, string Continent, IList<StayDto>? Stays);

public record StayDto(int Id, string Name, string Address, string City, double Rating, string ImageUrl);
