namespace Infrastructure.Logic

{
    // Holds singletons you want to share across forms
    public sealed class AppServices
    {
        public Core.Interfaces.IEmailSender EmailSender { get; }
        public Core.Interfaces.ISqlQueryRunner SqlQueryRunner { get; set; }  // settable so you can rebuild it after settings change
        public Core.Interfaces.IHangfireManager HangfireManager { get; }
        public Core.Interfaces.IFileSaver FileSaver { get; }
        public Infrastructure.Logic.Jobs.EmailJob EmailJob { get; }
        public Infrastructure.Logic.Templates.TemplateRenderer TemplateRenderer { get; }
        public Core.Interfaces.IReportRepository ReportRepository { get; }

        public AppServices(
            Core.Interfaces.IEmailSender emailSender,
            Core.Interfaces.ISqlQueryRunner sqlQueryRunner,
            Core.Interfaces.IHangfireManager hangfireManager,
            Core.Interfaces.IFileSaver fileSaver,
            Infrastructure.Logic.Jobs.EmailJob emailJob,
            Infrastructure.Logic.Templates.TemplateRenderer templateRenderer,
            Core.Interfaces.IReportRepository reportRepository)
        {
            EmailSender = emailSender;
            SqlQueryRunner = sqlQueryRunner;
            HangfireManager = hangfireManager;
            FileSaver = fileSaver;
            EmailJob = emailJob;
            TemplateRenderer = templateRenderer;
            ReportRepository = reportRepository;
        }
    }
}
