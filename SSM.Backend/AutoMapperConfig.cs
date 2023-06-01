using AutoMapper;
using SSM.Backend.Models;
using SSM.Backend.Models.Dto;

namespace SSM.Backend
{
    public class AutoMapperConfig:Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Department, DepartmentDTO>().ReverseMap();
            CreateMap<Department, DepartmentCreateDTO>().ReverseMap();
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
        }
    }
}
