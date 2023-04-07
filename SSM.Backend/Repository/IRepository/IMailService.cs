using SSM.Backend.Models;

namespace SSM.Backend.Repository.IRepository
{
    public interface IMailService
    {
        Task<bool> SendAsync(MailData mailData, CancellationToken ct);
    }
}
