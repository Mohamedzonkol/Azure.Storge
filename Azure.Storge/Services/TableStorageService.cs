using Azure.Storge.Data;

namespace Azure.Storge.Services
{
    public class TableStorageService : ITableStorageService
    {
        public Task DeleteAttendee(string industry, string id)
        {
            throw new NotImplementedException();
        }

        public Task<AttendeeEntity> GetAttendee(string industry, string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<AttendeeEntity>> GetAttendees()
        {
            throw new NotImplementedException();
        }

        public Task UpsertAttendee(AttendeeEntity attendeeEntity)
        {
            throw new NotImplementedException();
        }
    }
}
