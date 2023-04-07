using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
using ms_notifications_akinmueble.Models;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

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
            name=data.destinyName,
            message="Welcome to Akinmueble real estate."
        });
        var response = await client.SendEmailAsync(msg);
        if(response.StatusCode == System.Net.HttpStatusCode.Accepted){
            return Ok("Email sent to the address "+  data.destinyEmail);
        }else{
            return BadRequest("Error sending the message to the address: " + data.destinyEmail);
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
            name=data.destinyName,
            message="This is your new password... Do not share it."
        });
        var response = await client.SendEmailAsync(msg);
        if(response.StatusCode == System.Net.HttpStatusCode.Accepted){
            return Ok("Email sent to the address "+  data.destinyEmail);
        }else{
            return BadRequest("Error sending the message to the address: " + data.destinyEmail);
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
            name=data.destinyName,
            message=data.emailBody,
            subject=data.emailSubject
        });
        var response = await client.SendEmailAsync(msg);
        if(response.StatusCode == System.Net.HttpStatusCode.Accepted){
            return Ok("Email sent to the address "+  data.destinyEmail);
        }else{
            return BadRequest("Error sending the message to the address: " + data.destinyEmail);
        }
    }
    

    private SendGridMessage CreateBaseMessage(EmailModel data){
        
        var from = new EmailAddress(Environment.GetEnvironmentVariable("EMAIL_FROM"), Environment.GetEnvironmentVariable("NAME_FROM"));
        var subject = data.emailSubject;
        var to = new EmailAddress(data.destinyEmail, data.destinyName);
        var plainTextContent = data.emailBody;
        var htmlContent = data.emailBody;
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        return msg;
    }

    [Route ("send-new-password-sms")]
    [HttpPost]
    public async Task<ActionResult> SendNewPasswordSMS(SMSModel data){
        
        Console.Write("Server Activo");

        var accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY");
        var secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_KEY");
        var client = new AmazonSimpleNotificationServiceClient(accessKey, secretKey, RegionEndpoint.USEast1);
        var messageAttributes = new Dictionary<string, MessageAttributeValue>();
        var smsType = new MessageAttributeValue
        {
            DataType = "String",
            StringValue = "Transactional"
        };
        messageAttributes.Add("AWS.SNS.SMS.SMSType", smsType);

        Console.Write(accessKey + " " + secretKey);

        PublishRequest request = new PublishRequest{
            Message = data.smsBody,
            PhoneNumber = data.destinyPhone,
            MessageAttributes = messageAttributes
        };
        try
        {
            await client.PublishAsync(request);
            return Ok("SMS Sent");
        }
        catch
        {
            return BadRequest("Error sending SMS");
        }
    }
    
}