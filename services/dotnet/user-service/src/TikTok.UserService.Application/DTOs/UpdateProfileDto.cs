using TikTok.UserService.Domain.Enums;

namespace TikTok.UserService.Application.DTOs
{
    public class UpdateProfileDto
    {
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public bool? IsPrivate { get; set; }
    }
}
