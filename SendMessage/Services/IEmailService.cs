using SendMessage.DtosModel;

namespace SendMessage.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(MailView request);
        Task<ICollection<MailBody>> GetAllMailAsync();
        Task LogMessageAsync(MailView mail);
    }
}
