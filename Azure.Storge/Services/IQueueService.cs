using Azure.Storge.Models;

namespace Azure.Storge.Services
{
    public interface IQueueService
    {
        Task SendMessage(EmailMessage emailMessage);
    }
}
