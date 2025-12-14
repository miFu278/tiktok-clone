using System.Text.RegularExpressions;
using TikTok.Shared.Common.Abstractions.ValueObjects;

namespace TikTok.UserService.Domain.ValueObjects
{
    public sealed class Username : ValueObject
    {
        private const int MinLength = 3;
        private const int MaxLength = 30;
        private static readonly Regex UsernameRegex = new(
            @"^[a-zA-Z0-9._]+$",
            RegexOptions.Compiled);

        public string Value { get; private set; }

        private Username(string value)
        {
            Value = value;
        }

        public static Username Create(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));

            username = username.Trim().ToLowerInvariant();

            if (username.Length < MinLength)
                throw new ArgumentException($"Username must be at least {MinLength} characters", nameof(username));

            if (username.Length > MaxLength)
                throw new ArgumentException($"Username cannot exceed {MaxLength} characters", nameof(username));

            if (!UsernameRegex.IsMatch(username))
                throw new ArgumentException("Username can only contain letters, numbers, dots and underscores", nameof(username));

            if (username.StartsWith('.') || username.EndsWith('.'))
                throw new ArgumentException("Username cannot start or end with a dot", nameof(username));

            if (username.Contains(".."))
                throw new ArgumentException("Username cannot contain consecutive dots", nameof(username));

            return new Username(username);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;

        public static implicit operator string(Username username) => username.Value;
    }
}
