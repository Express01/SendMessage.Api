using SendMessage.SendMessage.Data.Models;

namespace SendMessage.SendMessage.Data.Repositories
{
    public interface IMailRepository
    {
        Task PostMailAsync(MailDAO entity);
        Task <ICollection<MailDAO>> GetMailsAsync();
    }
}
