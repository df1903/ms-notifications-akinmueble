using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
using ms_notifications_akinmueble.Models;

namespace ms_notifications_akinmueble.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationsController : ControllerBase
{
    [Route ("welcome-email")]
    [HttpPost]
    public async Task<ActionResult> SendWelcomeEmail(EmailModel data){

        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        var client = new SendGridClient(apiKey);

        SendGridMessage msg = this.CreateBaseMessage(data);
        msg.SetTemplateId(Environment.GetEnvironmentVariable("WELCOME_SENDGRID_TEMPLATE_ID"));
        msg.SetTemplateData(new
        {
            name=data.recipientName,
            message="Welcome to Akinmueble real estate."
        });
        var response = await client.SendEmailAsync(msg);
        if(response.StatusCode == System.Net.HttpStatusCode.Accepted){
            return Ok("Email sent to the address "+  data.recipientEmail);
        }else{
            return BadRequest("Error sending the message to the address: " + data.recipientEmail);
        }
    }


    [Route ("password-recovery-email")]
    [HttpPost]
    public async Task<ActionResult> SendPasswordRecoveryEmail(EmailModel data){
        
        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        var client = new SendGridClient(apiKey);

        SendGridMessage msg = this.CreateBaseMessage(data);
        msg.SetTemplateId(Environment.GetEnvironmentVariable("WELCOME_SENDGRID_TEMPLATE_ID"));
        msg.SetTemplateData(new
        {
            name=data.recipientName,
            message="This is your new password... Do not share it."
        });
        var response = await client.SendEmailAsync(msg);
        if(response.StatusCode == System.Net.HttpStatusCode.Accepted){
            return Ok("Email sent to the address "+  data.recipientEmail);
        }else{
            return BadRequest("Error sending the message to the address: " + data.recipientEmail);
        }
    }

    [Route ("send-code-2fa")]
    [HttpPost]
    public async Task<ActionResult> SendCode2faEmail(EmailModel data){
        
        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        var client = new SendGridClient(apiKey);

        SendGridMessage msg = this.CreateBaseMessage(data);
        msg.SetTemplateId(Environment.GetEnvironmentVariable("CODE2FA_SENDGRID_TEMPLATE_ID"));
        msg.SetTemplateData(new
        {
            name=data.recipientName,
            message=data.emailBody,
            subject=data.emailSubject
        });
        var response = await client.SendEmailAsync(msg);
        if(response.StatusCode == System.Net.HttpStatusCode.Accepted){
            return Ok("Email sent to the address "+  data.recipientEmail);
        }else{
            return BadRequest("Error sending the message to the address: " + data.recipientEmail);
        }
    }
    

    private SendGridMessage CreateBaseMessage(EmailModel data){
        
        var from = new EmailAddress(Environment.GetEnvironmentVariable("EMAIL_FROM"), Environment.GetEnvironmentVariable("NAME_FROM"));
        var subject = data.emailSubject;
        var to = new EmailAddress(data.recipientEmail, data.recipientName);
        var plainTextContent = data.emailBody;
        var htmlContent = data.emailBody;
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        return msg;
    }
}