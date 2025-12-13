namespace TikTok.UserService.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailVerificationAsync(string toEmail, string toName, string verificationToken, CancellationToken cancellationToken = default);
        Task SendPasswordResetAsync(string toEmail, string toName, string resetToken, CancellationToken cancellationToken = default);
        Task SendWelcomeEmailAsync(string toEmail, string toName, CancellationToken cancellationToken = default);
        Task SendEmailAsync(string toEmail, string toName, string subject, string htmlContent, CancellationToken cancellationToken = default);
    }
}
