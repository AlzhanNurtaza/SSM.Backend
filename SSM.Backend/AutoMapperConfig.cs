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
            CreateMap<Course, CourseDTO>().ReverseMap();
            CreateMap<Course, CourseCreateDTO>().ReverseMap();
            CreateMap<Speciality, SpecialityDTO>().ReverseMap();
            CreateMap<Speciality, SpecialityCreateDTO>().ReverseMap();
            CreateMap<Group, GroupDTO>().ReverseMap();
            CreateMap<Group, GroupCreateDTO>().ReverseMap();
            CreateMap<Classroom, ClassroomDTO>().ReverseMap();
            CreateMap<Classroom, ClassroomCreateDTO>().ReverseMap();
            CreateMap<Enrollment, EnrollmentDTO>().ReverseMap();
            CreateMap<Enrollment, EnrollmentCreateDTO>().ReverseMap();
            CreateMap<Schedule, ScheduleDTO>().ReverseMap();
            CreateMap<Schedule, ScheduleCreateDTO>().ReverseMap();
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
        }
    }
}
