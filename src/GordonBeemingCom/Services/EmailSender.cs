using Microsoft.AspNetCore.Identity.UI.Services;

namespace GordonBeemingCom.Services;

public class EmailSender : IEmailSender
{
  public Task SendEmailAsync(string email, string subject, string htmlMessage)
  {
    // DO NOTHING... BECAUSE WE ARE NOT SENDING EMAILS
    return Task.CompletedTask;
  }
}
