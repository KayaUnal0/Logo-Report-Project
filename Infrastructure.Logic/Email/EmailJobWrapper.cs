using Infrastructure.Logic.Jobs;

public static class EmailJobWrapper
{
    public static EmailJob JobInstance { get; set; }

    public static void SendEmail(string email, string subject, string body)
    {
        JobInstance?.Send(email, subject, body);
    }
}
