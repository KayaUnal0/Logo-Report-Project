namespace Core.Interfaces
{
    public interface IEmailSender
    {
        bool Send(string toAddress, string subject, string body, string attachmentPath = null);
    }
}