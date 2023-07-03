using Microsoft.EntityFrameworkCore;
using SendMessage.SendMessage.Data.Models;

namespace SendMessage.SendMessage.Data.Context
{
    public class MSSqlContext:DbContext
    {
        public MSSqlContext(DbContextOptions<MSSqlContext>options):base(options) 
        {
            
        }
        /** 
        1st table for logging sucsessfull or failed mail
       */
        public DbSet<MailDAO> LoggedMail { get; set; }
    }
}
