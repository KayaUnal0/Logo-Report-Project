using System;
using System.Linq.Expressions;
public interface IHangfireManager
{
    void Start();
    void Stop();
    void EnqueueEmail(string email, string subject, string body);
}
