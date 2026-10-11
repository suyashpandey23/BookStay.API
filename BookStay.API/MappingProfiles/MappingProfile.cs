using AutoMapper;
using BookStay.API.Data;
using BookStay.API.DTOs.Booking;
using BookStay.API.DTOs.Country;
using BookStay.API.DTOs.Stay;
using BookStay.API.Enums;

namespace BookStay.API.MappingProfiles;

/// <summary>
/// AutoMapper profile for Stay entity mappings.
/// 
/// General: Defines how Stay entities are mapped to and from DTOs.
/// 
/// Technical: AutoMapper Profile class that contains mapping configurations.
/// CreateMap defines the source and destination types for mapping.
/// </summary>
public class StayMappingProfile : Profile
{
    public StayMappingProfile()
    {
        CreateMap<Stay, GetStayDto>()
            .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.Country != null ? src.Country.Id : 0));

        CreateMap<CreateStayDto, Stay>();
    }
}

/// <summary>
/// AutoMapper profile for Country entity mappings.
/// 
/// General: Defines how Country entities are mapped to DTOs.
/// 
/// Technical: Simple one-way mapping from Country to GetCountryDto.
/// </summary>
public class CountryMappingProfile : Profile
{
    public CountryMappingProfile()
    {
        CreateMap<Country, GetCountryDto>();
    }
}

/// <summary>
/// AutoMapper profile for Booking entity mappings.
/// 
/// General: Defines how Booking entities are mapped to and from DTOs.
/// 
/// Technical: Includes mappings for CreateBookingDto, UpdateBookingDto, and GetBookingDto.
/// The GetBookingDto mapping includes custom configuration for the Status enum to string conversion.
/// </summary>
public class BookingMappingProfile : Profile
{
    public BookingMappingProfile()
    {
        CreateMap<CreateBookingDto, Booking>();
        CreateMap<UpdateBookingDto, Booking>();
        CreateMap<Booking, GetBookingDto>()
            .ForMember(dest => dest.StayName, opt => opt.MapFrom(src => src.Stay != null ? src.Stay.Name : string.Empty))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}
