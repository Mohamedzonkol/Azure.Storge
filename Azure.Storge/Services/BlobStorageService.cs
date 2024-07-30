using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace Azure.Storge.Services
{

    public class BlobStorageService(IConfiguration configuration, BlobServiceClient blobServiceClient) : IBlobStorageService
    {
        private string containerName = "attendeeimages";

        public async Task<string> GetBlobUrl(string imageName)
        {
            var container = blobServiceClient.GetBlobContainerClient(containerName);
            var blob = container.GetBlobClient(imageName);

            BlobSasBuilder blobSasBuilder = new()
            {
                BlobContainerName = blob.BlobContainerName,
                BlobName = blob.Name,
                ExpiresOn = DateTime.UtcNow.AddMinutes(2),
                Protocol = SasProtocol.Https,
                Resource = "b"
            };
            blobSasBuilder.SetPermissions(BlobAccountSasPermissions.Read);

            return blob.GenerateSasUri(blobSasBuilder).ToString();
        }

        public async Task RemoveBlob(string imageName)
        {
            var container = blobServiceClient.GetBlobContainerClient(containerName);
            var blob = container.GetBlobClient(imageName);
            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }

        public async Task<string> UploadBlob(IFormFile formFile, string imageName, string? originalBlobName = null)
        {
            var blobName = $"{imageName}{Path.GetExtension(formFile.FileName)}";
            var container = blobServiceClient.GetBlobContainerClient(containerName); ;

            if (!string.IsNullOrEmpty(originalBlobName))
            {
                await RemoveBlob(originalBlobName);
            }

            using var memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            var blob = container.GetBlobClient(blobName);
            await blob.UploadAsync(content: memoryStream, overwrite: true);
            return blobName;
        }
        private async Task<BlobContainerClient> GetBlobContainerClient()
        {
            try
            {
                BlobContainerClient container = new BlobContainerClient(configuration["StorageConnectionString"], containerName);
                await container.CreateIfNotExistsAsync();

                return container;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
