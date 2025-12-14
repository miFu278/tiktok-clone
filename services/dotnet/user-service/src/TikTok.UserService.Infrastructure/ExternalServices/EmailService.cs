using brevo_csharp.Api;
using brevo_csharp.Client;
using brevo_csharp.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TikTok.UserService.Application.Interfaces;

namespace TikTok.UserService.Infrastructure.ExternalServices
{
    public class BrevoEmailService : IEmailService
    {
        private readonly BrevoEmailOptions _options;
        private readonly ILogger<BrevoEmailService> _logger;
        private readonly TransactionalEmailsApi _emailApi;

        public BrevoEmailService(
            IOptions<BrevoEmailOptions> options,
            ILogger<BrevoEmailService> logger)
        {
            _options = options.Value;
            _logger = logger;

            // Configure Brevo API
            brevo_csharp.Client.Configuration.Default.AddApiKey("api-key", _options.ApiKey);
            _emailApi = new TransactionalEmailsApi();
        }

        public async System.Threading.Tasks.Task SendEmailVerificationAsync(
            string toEmail,
            string toName,
            string verificationToken,
            CancellationToken cancellationToken = default)
        {
            var verificationUrl = $"{_options.BaseUrl}/verify-email?token={verificationToken}";

            var htmlContent = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2>Welcome to TikTok Clone!</h2>
                        <p>Hi {toName},</p>
                        <p>Thank you for signing up. Please verify your email address by clicking the button below:</p>
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='{verificationUrl}' 
                               style='background-color: #fe2c55; color: white; padding: 12px 30px; 
                                      text-decoration: none; border-radius: 4px; display: inline-block;'>
                                Verify Email
                            </a>
                        </div>
                        <p>Or copy and paste this link into your browser:</p>
                        <p style='color: #666; word-break: break-all;'>{verificationUrl}</p>
                        <p>This link will expire in 24 hours.</p>
                        <p>If you didn't create an account, please ignore this email.</p>
                        <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                        <p style='color: #999; font-size: 12px;'>TikTok Clone Team</p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(toEmail, toName, "Verify Your Email Address", htmlContent, cancellationToken);
        }

        public async System.Threading.Tasks.Task SendPasswordResetAsync(
            string toEmail,
            string toName,
            string resetToken,
            CancellationToken cancellationToken = default)
        {
            var resetUrl = $"{_options.BaseUrl}/reset-password?token={resetToken}";

            var htmlContent = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2>Reset Your Password</h2>
                        <p>Hi {toName},</p>
                        <p>We received a request to reset your password. Click the button below to create a new password:</p>
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='{resetUrl}' 
                               style='background-color: #fe2c55; color: white; padding: 12px 30px; 
                                      text-decoration: none; border-radius: 4px; display: inline-block;'>
                                Reset Password
                            </a>
                        </div>
                        <p>Or copy and paste this link into your browser:</p>
                        <p style='color: #666; word-break: break-all;'>{resetUrl}</p>
                        <p>This link will expire in 1 hour.</p>
                        <p>If you didn't request a password reset, please ignore this email or contact support if you have concerns.</p>
                        <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                        <p style='color: #999; font-size: 12px;'>TikTok Clone Team</p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(toEmail, toName, "Reset Your Password", htmlContent, cancellationToken);
        }

        public async System.Threading.Tasks.Task SendWelcomeEmailAsync(
            string toEmail,
            string toName,
            CancellationToken cancellationToken = default)
        {
            var htmlContent = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2>Welcome to TikTok Clone! 🎉</h2>
                        <p>Hi {toName},</p>
                        <p>Your email has been verified successfully! You're all set to start creating and sharing amazing videos.</p>
                        <div style='margin: 30px 0;'>
                            <h3>Get Started:</h3>
                            <ul style='line-height: 1.8;'>
                                <li>Complete your profile</li>
                                <li>Follow interesting creators</li>
                                <li>Upload your first video</li>
                                <li>Explore trending content</li>
                            </ul>
                        </div>
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='{_options.BaseUrl}' 
                               style='background-color: #fe2c55; color: white; padding: 12px 30px; 
                                      text-decoration: none; border-radius: 4px; display: inline-block;'>
                                Start Exploring
                            </a>
                        </div>
                        <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                        <p style='color: #999; font-size: 12px;'>TikTok Clone Team</p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(toEmail, toName, "Welcome to TikTok Clone!", htmlContent, cancellationToken);
        }

        public async System.Threading.Tasks.Task SendEmailAsync(
            string toEmail,
            string toName,
            string subject,
            string htmlContent,
            CancellationToken cancellationToken = default)
        {
            var sendSmtpEmail = new SendSmtpEmail
            {
                Sender = new SendSmtpEmailSender
                {
                    Name = _options.SenderName,
                    Email = _options.SenderEmail
                },
                To = new List<SendSmtpEmailTo>
                {
                    new SendSmtpEmailTo
                    {
                        Email = toEmail,
                        Name = toName
                    }
                },
                Subject = subject,
                HtmlContent = htmlContent
            };

            try
            {
                var result = await _emailApi.SendTransacEmailAsync(sendSmtpEmail);
                _logger.LogInformation(
                    "Email sent successfully to {Email}. Message ID: {MessageId}",
                    toEmail,
                    result.MessageId);
            }
            catch (ApiException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send email to {Email}. Status: {Status}, Error: {Error}",
                    toEmail,
                    ex.ErrorCode,
                    ex.Message);
                throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
            }
        }
    }
}

