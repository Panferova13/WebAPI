using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;

namespace lrs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //get
            CreateMap<Company, CompanyDto>()
            .ForMember(c => c.FullAddress,
            opt => opt.MapFrom(x => string.Join(' ', x.Address, x.Country)));
            CreateMap<Employee, EmployeeDto>();
            CreateMap<City, CityDto>();
            CreateMap<Hotel, HotelDto>();
            //put
            CreateMap<CompanyForCreationDto, Company>();
            CreateMap<EmployeeForCreationDto, Employee>();
            CreateMap<CityCreateDto, City>();
            CreateMap<HotelCreateDto, Hotel>();
            CreateMap<UserForRegistrationDto, User>();
            //update
            CreateMap<CompanyForUpdateDto, Company>();
            CreateMap<EmployeeForUpdateDto, Employee>().ReverseMap();
            CreateMap<HotelUpdateDto, Hotel>().ReverseMap();
            CreateMap<CityUpdateDto, City>().ReverseMap();
        }
    }
}
