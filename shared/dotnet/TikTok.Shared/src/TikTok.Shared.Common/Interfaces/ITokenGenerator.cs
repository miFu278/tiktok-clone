namespace TikTok.Shared.Common.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateRandomToken(int length = 32);
        string GenerateSecureToken(int byteSize = 32);
        string GenerateNumericCode(int digits = 6);
        string GenerateUrlSafeToken(int length = 32);
        string GenerateRefreshToken();
        string GenerateVerificationToken();
        string GenerateResetPasswordToken();
        Guid GenerateGuid();
        string GenerateShortId(int length = 8);
    }
}
