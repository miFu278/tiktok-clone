using Google.Protobuf.WellKnownTypes;
using TikTok.Shared.Protos;
using TikTok.Shared.Protos.UserService;
using TikTok.UserService.Application.DTOs;
using DomainGender = TikTok.UserService.Domain.Enums.Gender;
using ProtoGender = TikTok.Shared.Protos.Gender;

namespace TikTok.UserService.API.Mappers
{
    public static class GrpcMapper
    {
        // DTO to Proto
        public static UserMessage ToProto(this UserDto dto)
        {
            var message = new UserMessage
            {
                Id = dto.Id.ToString(),
                Email = dto.Email,
                Username = dto.UserName ?? string.Empty,
                FullName = dto.FullName ?? string.Empty,
                Bio = dto.Bio ?? string.Empty,
                AvatarUrl = dto.AvatarUrl ?? string.Empty,
                DateOfBirth = dto.DateOfBirth.ToString("O"),
                Gender = dto.Gender.ToProtoGender(),
                EmailVerified = dto.EmailVerified,
                IsPrivate = dto.IsPrivate,
                IsActive = dto.IsActive,
                CreatedAt = dto.CreatedAt.ToString("O")
            };

            if (dto.Stats != null)
            {
                message.Stats = dto.Stats.ToProto();
            }

            message.Roles.AddRange(dto.Roles);

            return message;
        }

        public static UserStatsMessage ToProto(this UserStatsDto dto)
        {
            return new UserStatsMessage
            {
                FollowersCount = dto.FollowersCount,
                FollowingCount = dto.FollowingCount,
                VideosCount = dto.VideosCount,
                TotalLikesReceived = dto.TotalLikesReceived
            };
        }

        public static UserProfileMessage ToProto(this UserProfileDto dto)
        {
            return new UserProfileMessage
            {
                Id = dto.Id.ToString(),
                Username = dto.UserName ?? string.Empty,
                FullName = dto.FullName ?? string.Empty,
                Bio = dto.Bio ?? string.Empty,
                AvatarUrl = dto.AvatarUrl ?? string.Empty,
                Gender = dto.Gender.ToProtoGender(),
                IsPrivate = dto.IsPrivate,
                IsFollowing = dto.IsFollowing,
                IsFollowedBy = dto.IsFollowedBy,
                Stats = dto.Stats.ToProto()
            };
        }

        public static UserListItemMessage ToProto(this UserListItemDto dto)
        {
            return new UserListItemMessage
            {
                Id = dto.Id.ToString(),
                Username = dto.UserName ?? string.Empty,
                FullName = dto.FullName ?? string.Empty,
                AvatarUrl = dto.AvatarUrl ?? string.Empty,
                IsFollowing = dto.IsFollowing,
                FollowersCount = dto.FollowersCount
            };
        }

        public static UserSettingsResponse ToProto(this UserSettingsDto dto)
        {
            return new UserSettingsResponse
            {
                IsPrivateAccount = dto.IsPrivateAccount,
                AllowComments = dto.AllowComments,
                AllowDuet = dto.AllowDuet,
                AllowStitch = dto.AllowStitch,
                AllowDownload = dto.AllowDownload,
                PushNotificationsEnabled = dto.PushNotificationsEnabled,
                EmailNotificationsEnabled = dto.EmailNotificationsEnabled,
                NotifyOnLikes = dto.NotifyOnLikes,
                NotifyOnComments = dto.NotifyOnComments,
                NotifyOnFollows = dto.NotifyOnFollows,
                NotifyOnMentions = dto.NotifyOnMentions,
                PreferredLanguage = dto.PreferredLanguage,
                PreferredContentRegion = dto.PreferredContentRegion
            };
        }

        // Proto to DTO
        public static RegisterDto ToDto(this RegisterRequest request)
        {
            return new RegisterDto
            {
                Email = TikTok.UserService.Domain.ValueObjects.Email.Create(request.Email),
                Password = request.Password,
                ConfirmPassword = request.ConfirmPassword,
                UserName = TikTok.UserService.Domain.ValueObjects.Username.Create(request.Username),
                FullName = request.FullName,
                DateOfBirth = DateTime.Parse(request.DateOfBirth),
                Gender = request.Gender.ToDomainGender()
            };
        }

        public static LoginDto ToDto(this LoginRequest request)
        {
            return new LoginDto
            {
                Email = request.Email,
                Password = request.Password
            };
        }

        public static UpdateProfileDto ToDto(this UpdateProfileRequest request)
        {
            return new UpdateProfileDto
            {
                UserName = request.Username,
                FullName = request.FullName,
                Bio = request.Bio,
                AvatarUrl = request.AvatarUrl,
                DateOfBirth = string.IsNullOrEmpty(request.DateOfBirth) ? null : DateTime.Parse(request.DateOfBirth),
                Gender = request.HasGender ? request.Gender.ToDomainGender() : null,
                IsPrivate = request.HasIsPrivate ? request.IsPrivate : null
            };
        }

        public static ChangePasswordDto ToDto(this ChangePasswordRequest request)
        {
            return new ChangePasswordDto
            {
                CurrentPassword = request.CurrentPassword,
                NewPassword = request.NewPassword,
                ConfirmNewPassword = request.ConfirmNewPassword
            };
        }

        public static UserSettingsDto ToDto(this UpdateUserSettingsRequest request)
        {
            return new UserSettingsDto
            {
                IsPrivateAccount = request.IsPrivateAccount,
                AllowComments = request.AllowComments,
                AllowDuet = request.AllowDuet,
                AllowStitch = request.AllowStitch,
                AllowDownload = request.AllowDownload,
                PushNotificationsEnabled = request.PushNotificationsEnabled,
                EmailNotificationsEnabled = request.EmailNotificationsEnabled,
                NotifyOnLikes = request.NotifyOnLikes,
                NotifyOnComments = request.NotifyOnComments,
                NotifyOnFollows = request.NotifyOnFollows,
                NotifyOnMentions = request.NotifyOnMentions,
                PreferredLanguage = request.PreferredLanguage,
                PreferredContentRegion = request.PreferredContentRegion
            };
        }

        // Enum conversions
        public static ProtoGender ToProtoGender(this DomainGender gender)
        {
            return gender switch
            {
                DomainGender.Male => ProtoGender.Male,
                DomainGender.Female => ProtoGender.Female,
                DomainGender.Other => ProtoGender.Other,
                DomainGender.PreferNotToSay => ProtoGender.PreferNotToSay,
                _ => ProtoGender.Unspecified
            };
        }

        public static DomainGender ToDomainGender(this ProtoGender gender)
        {
            return gender switch
            {
                ProtoGender.Male => DomainGender.Male,
                ProtoGender.Female => DomainGender.Female,
                ProtoGender.Other => DomainGender.Other,
                ProtoGender.PreferNotToSay => DomainGender.PreferNotToSay,
                _ => DomainGender.PreferNotToSay
            };
        }
    }
}
