using TikTok.Shared.Common.Abstractions.Entities;
using TikTok.UserService.Domain.Enums;

namespace TikTok.UserService.Domain.Entities
{
    public class User : SoftDeletableEntity
    {
        public string Email { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? PasswordHash { get; set; }
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }

        // Email verification
        public bool EmailVerified { get; set; }
        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationTokenExpires { get; set; }

        // Password reset
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpires { get; set; }

        // Account status
        public bool IsPrivate { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsLocked { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public DateTime LastLoginAt { get; set; }

        // Navigate Properties
        public ICollection<UserRole> UserRoles { get; set; } = [];

        // Computed property
        public bool IsEmailVerificationTokenValid =>
            !string.IsNullOrEmpty(EmailVerificationToken) &&
            EmailVerificationTokenExpires.HasValue &&
            EmailVerificationTokenExpires.Value > DateTime.UtcNow;

        public bool IsPasswordResetTokenValid =>
            !string.IsNullOrEmpty(PasswordResetToken) &&
            PasswordResetTokenExpires.HasValue &&
            PasswordResetTokenExpires.Value > DateTime.UtcNow;

        public bool IsLockedOut =>
            IsLocked && LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;

        public bool CanLogin =>
            IsActive && !IsLockedOut && !IsDeleted;

        // Helper methods
        public void IncrementFailedLoginAttempts(int maxAttempts = 5, int lockoutMinutes = 15)
        {
            FailedLoginAttempts++;
            if (FailedLoginAttempts >= maxAttempts)
            {
                IsLocked = true;
                LockoutEnd = DateTime.UtcNow.AddMinutes(lockoutMinutes);
            }
        }

        public void ResetFailedLoginAttempts()
        {
            FailedLoginAttempts = 0;
            IsLocked = false;
            LockoutEnd = null;
            LastLoginAt = DateTime.UtcNow;
        }

        public void UnlockAccount()
        {
            IsLocked = false;
            LockoutEnd = null;
            FailedLoginAttempts = 0;
        }
    }
}
