namespace AspNetCoreIdentity.App.Service.Services
{
    public interface IEmailService
    {
        Task SendResetPasswordEmailAsync(string resetPasswordEmailLink, string to);
    }
}
