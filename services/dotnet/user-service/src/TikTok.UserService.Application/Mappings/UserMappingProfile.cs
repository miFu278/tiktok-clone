using AutoMapper;
using TikTok.UserService.Application.DTOs;
using TikTok.UserService.Domain.Entities;

namespace TikTok.UserService.Application.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            // User mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name)));

            CreateMap<User, UserProfileDto>()
                .ForMember(dest => dest.Stats, opt => opt.MapFrom(src => src.Stats))
                .ForMember(dest => dest.IsFollowing, opt => opt.Ignore())
                .ForMember(dest => dest.IsFollowedBy, opt => opt.Ignore());

            CreateMap<User, UserListItemDto>()
                .ForMember(dest => dest.FollowersCount, opt => opt.MapFrom(src => src.Stats != null ? src.Stats.FollowersCount : 0))
                .ForMember(dest => dest.IsFollowing, opt => opt.Ignore());

            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.EmailVerified, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<UpdateProfileDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // UserStats mappings
            CreateMap<UserStats, UserStatsDto>();

            // UserSettings mappings
            CreateMap<UserSettings, UserSettingsDto>();
            CreateMap<UserSettingsDto, UserSettings>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());
        }
    }
}
