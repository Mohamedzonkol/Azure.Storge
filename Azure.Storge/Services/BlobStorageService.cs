namespace Azure.Storge.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        public Task<string> GetBlobUrl(string imageName)
        {
            throw new NotImplementedException();
        }

        public Task RemoveBlob(string imageName)
        {
            throw new NotImplementedException();
        }

        public Task<string> UploadBlob(IFormFile formFile, string imageName, string? originalBlobName = null)
        {
            throw new NotImplementedException();
        }
    }
}
