using AutoMapper;
using Register_Login_Elasticsearch.DTOs;
using Register_Login_Elasticsearch.Models;

namespace Register_Login_Elasticsearch.AutoMapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Users, UserDto>().ReverseMap();
            CreateMap<Users, UserCreateDto>().ReverseMap();
            CreateMap<Users, UsersUpdateDto>().ReverseMap();
            CreateMap<Users, UserLoginDto>().ReverseMap();
            CreateMap<ResponseDto.LoginResult, UserLoginDto>().ReverseMap();
        }
    }
}
