namespace Core.Interfaces
{
    public interface IEmailSender
    {
        public bool Send(string toAddress, string subject, string body, string[] attachmentPaths = null);

    }
}