using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SendMessage.DtosModel;
using SendMessage.Services;

namespace SendMessage.Controllers
{
    [Route("api/mails")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        public MailController(IEmailService emailService)
        {
            _emailService = emailService;
        }
        /** <summary>
     * Form new Mail With one or multiple recipients    
     * </summary> <param name="request"></param>   
     */
        [HttpPost]
        public async Task<IActionResult> SendEmailAsync(MailView request)
        {
            if (request.Subject == null || request.Body == null || request.Recipients == null)
            {
                string message = "Failed to fill all Required Fields!";
                await _emailService.LogMessageAsync(request);
                return BadRequest(
                   new { Message = message });
            }
            await _emailService.SendEmailAsync(request);
            await _emailService.LogMessageAsync(request);
            return Ok();
        }
        /** 
       * <summary>To Get All Messages that was sent earlier. </summary>        
       */
        [HttpGet]
        public async Task<IActionResult> GetAllMailAsync()
        {
            ICollection<MailBody> result= await _emailService.GetAllMailAsync();
            return Ok(result);
        }
    }
}
