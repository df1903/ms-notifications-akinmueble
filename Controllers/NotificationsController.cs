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
    public async Task<ActionResult> SendWelcomeEmail(EmailModel datos){

        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        var client = new SendGridClient(apiKey);

        SendGridMessage msg = this.CreateBaseMessage(datos);
        msg.SetTemplateId(Environment.GetEnvironmentVariable("WELCOME_SENDGRID_TEMPLATE_ID"));
        msg.SetTemplateData(new
        {
            name=datos.recipientName,
            message="Welcome to Akinmueble real estate."
        });
        var response = await client.SendEmailAsync(msg);
        if(response.StatusCode == System.Net.HttpStatusCode.Accepted){
            return Ok("Email sent to the address "+  datos.recipientEmail);
        }else{
            return BadRequest("Error sending the message to the address: " + datos.recipientEmail);
        }
    }


    [Route ("password-recovery-email")]
    [HttpPost]
    public async Task<ActionResult> SendPasswordRecoveryEmail(EmailModel datos){
        
        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        var client = new SendGridClient(apiKey);

        SendGridMessage msg = this.CreateBaseMessage(datos);
        msg.SetTemplateId(Environment.GetEnvironmentVariable("WELCOME_SENDGRID_TEMPLATE_ID"));
        msg.SetTemplateData(new
        {
            name=datos.recipientName,
            message="This is your new password... Do not share it."
        });
        var response = await client.SendEmailAsync(msg);
        if(response.StatusCode == System.Net.HttpStatusCode.Accepted){
            return Ok("Email sent to the address "+  datos.recipientEmail);
        }else{
            return BadRequest("Error sending the message to the address: " + datos.recipientEmail);
        }
    }

    private SendGridMessage CreateBaseMessage(EmailModel datos){
        
        var from = new EmailAddress(Environment.GetEnvironmentVariable("EMAIL_FROM"), Environment.GetEnvironmentVariable("NAME_FROM"));
        var subject = datos.emailSubject;
        var to = new EmailAddress(datos.recipientEmail, datos.recipientName);
        var plainTextContent = "plain text content";
        var htmlContent = datos.emailBody;
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        return msg;
    }
}