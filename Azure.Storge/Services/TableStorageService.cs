using Azure.Data.Tables;
using Azure.Storge.Data;

namespace Azure.Storge.Services
{
    public class TableStorageService(IConfiguration configuration, TableServiceClient tableService, TableClient tableClient) : ITableStorageService
    {
        private const string tableName = "Attendees";
        public async Task DeleteAttendee(string industry, string id)
        {
            await tableClient.DeleteEntityAsync(industry, id);
        }

        public async Task<AttendeeEntity> GetAttendee(string industry, string id)
        {
            return await tableClient.GetEntityAsync<AttendeeEntity>(industry, id);
        }

        public async Task<List<AttendeeEntity>> GetAttendees()
        {
            return tableClient.Query<AttendeeEntity>().ToList();
        }

        public async Task UpsertAttendee(AttendeeEntity attendeeEntity)
        {
            await tableClient.UpsertEntityAsync(attendeeEntity);
        }
        private async Task<TableClient> GetTableClient()
        {
            var tableClient = tableService.GetTableClient(tableName);
            await tableClient.CreateIfNotExistsAsync();

            return tableClient;
        }
    }
}
