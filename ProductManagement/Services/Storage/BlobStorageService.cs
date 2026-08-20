using Azure.Identity;
using Azure.Storage.Blobs;

namespace ProductManagement.Services.Storage
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobStorageService(IConfiguration configuration)
        {
            var accountName = configuration["AzureStorage:AccountName"];
            var containerName = configuration["AzureStorage:ContainerName"];

            var serviceClient = new BlobServiceClient(
                new Uri($"https://{accountName}.blob.core.windows.net"),
                new DefaultAzureCredential());

            _containerClient = serviceClient.GetBlobContainerClient(containerName);
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);

            var fileName = $"{Guid.NewGuid()}{extension}";

            var blobClient = _containerClient.GetBlobClient(fileName);

            await using var stream = file.OpenReadStream();

            await blobClient.UploadAsync(stream, overwrite: false);

            return blobClient.Uri.ToString();
        }
    }
}