using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
using ms_notifications_akinmueble.Models;

namespace ms_notifications_akinmueble.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationsController : ControllerBase
{
    [Route ("email")]
    [HttpPost]
    public async Task<ActionResult> SendEmail(EmailModel datos){

        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress("lina.nieto22243@ucaldas.edu.co", "Lina Nieto");
        var subject = datos.emailSubject;
        var to = new EmailAddress(datos.recipientEmail, datos.recipientName);
        var plainTextContent = "plain text content";
        var htmlContent = datos.emailBody;
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync(msg);

        if(response.StatusCode == System.Net.HttpStatusCode.Accepted){
            return Ok("Email sent to the address "+  datos.recipientEmail);
        }else{
            return BadRequest("Error sending the message to the address: " + datos.recipientEmail);
        }
    }
}