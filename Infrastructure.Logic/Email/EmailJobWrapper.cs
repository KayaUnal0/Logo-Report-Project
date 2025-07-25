using Infrastructure.Logic.Email;
using Infrastructure.Logic.Jobs;

public static class EmailJobWrapper
{
    public static void SendEmail(string email, string subject, string body)
    {
        var sender = new EmailSender();
        var job = new EmailJob(sender);
        job.Send(email, subject, body);
    }
}
