using Hangfire;
using Hangfire.MemoryStorage;
using Serilog;
using Core.Interfaces;
using Infrastructure.Logic.Jobs;
using System;
using System.Linq.Expressions;
using Infrastructure.Logic.Email;

namespace Infrastructure.Logic.Hangfire
{
    public class HangfireServerManager : IHangfireManager
    {
        private BackgroundJobServer? _server;
        private readonly EmailJob _emailJob; 

        public HangfireServerManager(EmailJob emailJob)
        {
            _emailJob = emailJob;
        }

        public void Start()
        {
            GlobalConfiguration.Configuration
                .UseMemoryStorage();

            _server = new BackgroundJobServer();
            Log.Information("Hangfire server started with in-memory storage.");
        }

        public void Stop()
        {
            _server?.Dispose();
            Log.Information("Hangfire server stopped.");
        }

        public void Enqueue(Expression<Action> methodCall)
        {
            BackgroundJob.Enqueue(methodCall);
            Log.Information("Job enqueued to Hangfire.");
        }

        public void EnqueueEmail(string email, string subject, string body)
        {
            // Call a static method that manually resolves the dependency
            BackgroundJob.Enqueue(() => EmailJobWrapper.SendEmail(email, subject, body));
        }


    }
}
