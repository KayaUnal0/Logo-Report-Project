namespace Core.Interfaces
{
    public interface IJobRunner
    {
        void SubmitReportJob(string email, string subject, string body);
    }
}
