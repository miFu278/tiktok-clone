using TikTok.UserService.Domain.Enums;
using TikTok.UserService.Domain.ValueObjects;

namespace TikTok.UserService.Application.DTOs
{
    public class RegisterDto
    {
        public Email Email { get; set; } = null!;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public Username UserName { get; set; } = null!;
        public string? FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
    }
}
