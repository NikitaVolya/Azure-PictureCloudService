using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using PictureCloudService.Models;

namespace PictureCloudService.Services
{
    public class PictureBlobStorage
    {
        private static readonly string _containerName = "picture-cloud";

        private string _connectionString;
        private BlobServiceClient _serviceClient;
        private BlobContainerClient _containerClient;

        public PictureBlobStorage()
        {
            _connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING") ?? throw new ArgumentNullException("AZURE_STORAGE_CONNECTION_STRING is not set");
            _serviceClient = new BlobServiceClient(_connectionString);

            _containerClient = _serviceClient.GetBlobContainerClient(_containerName);
            _containerClient.CreateIfNotExists();
        }

        public async Task UploadPictureAsync(Picture picture, Stream fileStream)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(picture.VirtualPath);
            await blobClient.UploadAsync(fileStream);
        }

        public async Task DeletePictureAsync(Picture picture)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(picture.VirtualPath);
            await blobClient.DeleteAsync();
        }

        public string GetHref(Picture picture)
        {
            BlobSasBuilder sas = new BlobSasBuilder()
            {
                BlobContainerName = _containerName,
                BlobName = picture.VirtualPath,
                Resource = "b",
                ExpiresOn = DateTime.UtcNow.AddHours(1)
            };
            sas.SetPermissions(BlobSasPermissions.Read);

            BlobClient blobClient = _containerClient.GetBlobClient(picture.VirtualPath);
            Uri sasUri = blobClient.GenerateSasUri(sas);

            return sasUri.ToString();
        }
    }
}
