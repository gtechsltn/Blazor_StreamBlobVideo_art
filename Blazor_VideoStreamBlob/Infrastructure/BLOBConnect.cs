using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace Blazor_VideoStreamBlob.Infrastructure
{
    public class BLOBConnect(IConfiguration configuration)
    {
        private readonly string? _connectionString = configuration.GetSection("AzureStorageSettings:BLOB_CONNECTION_STRING").Value;
        private readonly string? _containerName = configuration.GetSection("AzureStorageSettings:BLOB_CONTAINER_NAME").Value;


        public async Task<string> GetBlobSasToken(string blobName)
        {
            var blobSvcClient = new BlobServiceClient(_connectionString);
            var containerClient = blobSvcClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var sasBuilder = new BlobSasBuilder(BlobContainerSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(15))
            {
                BlobContainerName = _containerName,
                BlobName = blobName,
                Resource = "b",
                //StartsOn = DateTimeOffset.UtcNow.AddMinutes(-1),
                //ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(15)
            };
            await containerClient.SetAccessPolicyAsync(PublicAccessType.None);
            //sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasToken = blobClient.GenerateSasUri(sasBuilder).Query;
            return await Task.FromResult( $"{blobClient.Uri}{sasToken}");
        }

        // code to list all blobs in a container
        public List<string> ListBlobs()
        {
            var blobSvcClient = new BlobServiceClient(_connectionString);
            var containerClient = blobSvcClient.GetBlobContainerClient(_containerName);
            var blobs = containerClient.GetBlobs().Select(b => b.Name).Where(b => b.EndsWith(".mp4")).ToList();
            return blobs;
        }


        public async Task<Stream> GetBlobFileStream(string blobName)
        {
            var blobSvcClient = new BlobServiceClient(_connectionString);
            var containerClient = blobSvcClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            return await blobClient.OpenReadAsync();
        }
    }
}
