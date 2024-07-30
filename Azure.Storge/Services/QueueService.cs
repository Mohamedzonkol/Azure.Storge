using Azure.Storage.Queues;
using Azure.Storge.Models;
using Newtonsoft.Json;

namespace Azure.Storge.Services
{
    public class QueueService(IConfiguration configuration, QueueClient queueClient) : IQueueService
    {
        public async Task SendMessage(EmailMessage emailMessage)
        {
            //var queueClient = new QueueClient(_configuration["StorageConnectionString"],
            //    queueName,
            //    new QueueClientOptions
            //    {
            //        MessageEncoding = QueueMessageEncoding.Base64
            //    });

            await queueClient.CreateIfNotExistsAsync();

            var message = JsonConvert.SerializeObject(emailMessage);

            await queueClient.SendMessageAsync(message);
        }
    }
}
