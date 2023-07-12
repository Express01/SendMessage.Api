using AutoMapper;
using MailKit.Security;
using Microsoft.OpenApi.Extensions;
using MimeKit;
using MimeKit.Text;
using SendMessage.DtosModel;
using SendMessage.SendMessage.Data.Models;
using SendMessage.SendMessage.Data.Repositories;
using System.Net.Mail;
using System.Text;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace SendMessage.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly IMailRepository _mailRepository;
        private IMapper _mapper;
        private MapperConfiguration mapperConfiguration;
        private string failedMessage;
        private MailBody.SendResult status = MailBody.SendResult.Failed;
        public EmailService(IConfiguration config,IMailRepository mailRepository)
        {
            _config = config;
            _mailRepository = mailRepository;
            /**
            * Automapper also allows us to ignore some of the fields 
           * when mapped in one of the direction if you want to hide some info            
           */
            mapperConfiguration = new MapperConfiguration(cfq =>
            {
                cfq.CreateMap<MailBody, MailDAO>().ForMember(x => x.Id, opt => opt.Ignore());
                cfq.CreateMap<MailDAO, MailBody>();
            });
            _mapper = new Mapper(mapperConfiguration);
        }
        /** <summary>
       * Returns new Collection of logged mails from database  </summary>  */
        public async Task<ICollection<MailBody>> GetAllMailAsync()
        {
            ICollection<MailDAO> temp = await _mailRepository.GetMailsAsync();
            ICollection<MailBody> result = _mapper.Map<ICollection<MailDAO>, ICollection<MailBody>>(temp);
            return result;
        }
        /** <summary>
        * Log requested mail to DB, sucsessful of not.  </summary>  */
        public async Task LogMessageAsync(MailView mail)
        {
            MailBody body = new()
            {
                Subject = mail.Subject,
                Recipients = String.Join(',', mail.Recipients),
                Body = mail.Body,
                Date = DateTime.Now,
                Result = status,
                FailedMessage = failedMessage
            };
            MailDAO mailDAO=_mapper.Map<MailDAO>(body);
            await _mailRepository.PostMailAsync(mailDAO);

        }

        /** <summary> Forming new email's according to config file  </summary>      
     */
        public async Task SendEmailAsync(MailView request)
        {
            MimeMessage email = new();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
            email.Subject=request.Subject;
            email.Body=new TextPart(TextFormat.Html) { Text = request.Body };
            using (MailKit.Net.Smtp.SmtpClient smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                smtp.CheckCertificateRevocation = false;
                await smtp.ConnectAsync(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
                try
                {
                    foreach (string item in request.Recipients)
                    {
                        email.To.Clear();
                        email.To.Add(MailboxAddress.Parse(item));
                        await smtp.SendAsync(email);
                        Task.Delay(100).Wait();
                        status = MailBody.SendResult.Ok ;
                    }
                }
                catch (Exception ex)
                {
                    failedMessage = ex.Message;
                    status = MailBody.SendResult.Failed;
                }
                finally
                {
                    smtp.Disconnect(true);
                    smtp.Dispose();
                }
            }
        }
    }
}
