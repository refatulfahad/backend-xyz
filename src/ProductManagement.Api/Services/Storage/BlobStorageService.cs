using Azure.Identity;
using Azure.Storage.Blobs;

namespace ProductManagement.Services.Storage
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _containerClient;

        private readonly string _cdnBaseUrl;
        private readonly string _containerName;
        private readonly string _accountName;

        public BlobStorageService(IConfiguration configuration)
        {
            _accountName = configuration["AzureStorage:AccountName"];
            _containerName = configuration["AzureStorage:ContainerName"] ?? throw new InvalidOperationException(
        "AzureStorage:ContainerName is not configured.");
            _cdnBaseUrl = configuration["Cdn:BaseUrl"] ?? throw new InvalidOperationException(
        "Cdn:BaseUrl is not configured.");

            var serviceClient = new BlobServiceClient(
                new Uri($"https://{_accountName}.blob.core.windows.net"),
                new DefaultAzureCredential());

            _containerClient = serviceClient.GetBlobContainerClient(_containerName);
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);

            var fileName = $"{Guid.NewGuid()}{extension}";

            var blobClient = _containerClient.GetBlobClient(fileName);

            await using var stream = file.OpenReadStream();

            await blobClient.UploadAsync(stream, overwrite: false);

            return $"https://{_accountName}.blob.core.windows.net/{_containerName}/{fileName}";
        }
    }
}