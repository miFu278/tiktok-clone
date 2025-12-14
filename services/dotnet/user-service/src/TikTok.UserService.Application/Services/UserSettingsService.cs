using AutoMapper;
using TikTok.Shared.Common.Exceptions;
using TikTok.UserService.Application.DTOs;
using TikTok.UserService.Application.Interfaces;
using TikTok.UserService.Domain.Entities;
using TikTok.UserService.Domain.Interfaces;

namespace TikTok.UserService.Application.Services
{
    public class UserSettingsService : IUserSettingsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserSettingsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserSettingsDto?> GetUserSettingsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var settings = await _unitOfWork.UserSettings.GetByUserIdAsync(userId, cancellationToken);
            return settings != null ? _mapper.Map<UserSettingsDto>(settings) : null;
        }

        public async Task<UserSettingsDto> UpdateUserSettingsAsync(Guid userId, UserSettingsDto dto, CancellationToken cancellationToken = default)
        {
            var settings = await _unitOfWork.UserSettings.GetByUserIdAsync(userId, cancellationToken);

            if (settings == null)
            {
                throw new NotFoundException("User settings not found");
            }

            _mapper.Map(dto, settings);
            settings.SetUpdated(userId);

            await _unitOfWork.UserSettings.UpdateAsync(settings, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<UserSettingsDto>(settings);
        }

        public async Task<UserSettingsDto> CreateDefaultSettingsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var settings = new UserSettings
            {
                UserId = userId,
                IsPrivateAccount = false,
                AllowComments = true,
                AllowDuet = true,
                AllowStitch = true,
                AllowDownload = true,
                PushNotificationsEnabled = true,
                EmailNotificationsEnabled = true,
                NotifyOnLikes = true,
                NotifyOnComments = true,
                NotifyOnFollows = true,
                NotifyOnMentions = true,
                PreferredLanguage = "en",
                PreferredContentRegion = "US"
            };

            await _unitOfWork.UserSettings.AddAsync(settings, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<UserSettingsDto>(settings);
        }
    }
}
