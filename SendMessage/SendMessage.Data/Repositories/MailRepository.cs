using Microsoft.EntityFrameworkCore;
using SendMessage.SendMessage.Data.Context;
using SendMessage.SendMessage.Data.Models;

namespace SendMessage.SendMessage.Data.Repositories
{
    public class MailRepository : IMailRepository
    {
        private readonly MSSqlContext _context;
        public MailRepository(MSSqlContext context)
        {
            _context = context;
        }
        /**
        * <summary> Asynchronously creates ICollection<MailDAO> from database log </summary>      
       */
        public async Task<ICollection<MailDAO>> GetMailsAsync()
        {
            var result = await _context.LoggedMail.OrderBy(x => x).ToListAsync();
            return result;
        }

        public async Task PostMailAsync(MailDAO entity)
        {
            _context.LoggedMail.Add(entity);
            await _context.SaveChangesAsync();
        }
    }
}
