using LIN.Hangfire.Services;

namespace LIN.Hangfire.Jobs;

public class MailSenderJob(EmailService emailService)
{

    /// <summary>
    /// Ejecutar el job.
    /// </summary>
    public async Task Run(string mail, string subject, string content)
    {
        await emailService.SendMail([mail], "security", subject, content);

    }

}