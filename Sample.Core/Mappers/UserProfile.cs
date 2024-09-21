using AutoMapper;
using Sample.Common.DTOs.Requests;
using Sample.Common.DTOs.Responses;
using Sample.Data.Entities;

namespace Sample.Core.Mappers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserRequest, User>();
            CreateMap<User, UserResponse>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Description))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Role.Permissions.Select(p => p.Description)));
        }
    }
}