using AutoMapper;
using BookStay.API.Data;
using BookStay.API.DTOs.Country;
using BookStay.API.DTOs.Stay;

namespace BookStay.API.MappingProfiles;

public class StayMappingProfile: Profile {

    public StayMappingProfile() {
        CreateMap<Stay,GetStayDto>().
            ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.Country != null ? src.Country.Id : 0));

        CreateMap<CreateStayDto,Stay>();
    }
    
}

public class CountryMappingProfile: Profile {
    public CountryMappingProfile() {
        CreateMap<Country,GetCountryDto>();
    }
}
