
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureBlobStorageDemo.Models;
using System;

namespace AzureBlobStorageDemo.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<bool> DeleteBlob(string containerName, string blobName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

            BlobModel blob = new();

            if (await blobClient.ExistsAsync())
            {
                await blobContainerClient.DeleteBlobAsync(blobName);

                return true;
            }

            return false;
        }

        public async Task<FormFile?> DownloadBlob(string containerName, string blobName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

            BlobModel blob = new();

            if (await blobClient.ExistsAsync())
            {
                Stream stream = new MemoryStream();

                var response = await blobClient.DownloadToAsync(stream);

                // Reset the stream position to the beginning
                stream.Position = 0;

                FormFile formFile = new FormFile(
                    baseStream: stream,
                    baseStreamOffset: 0,
                    length: stream.Length,
                    name: "file",
                    fileName: blobName
                )
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "application/octet-stream"
                };

                return formFile;
            }

            return null;
        }

        public async Task<List<BlobModel>> GetAllBlobs(string containerName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            List<BlobModel> blobs = new List<BlobModel>();

            await foreach (BlobItem blobItem in blobContainerClient.GetBlobsAsync())
            {
                BlobModel blob = new()
                {
                    Name = blobItem.Name,
                    CreatedOn = blobItem.Properties.CreatedOn
                };

                blobs.Add(blob);
            }

            return blobs;
        }

        public async Task<BlobModel> GetBlob(string containerName, string blobName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

            BlobModel blob = new();

            if (await blobClient.ExistsAsync())
            {
                blob.Name = blobClient.Name;
                blob.URL = blobClient.Uri.AbsoluteUri;
            }

            return blob;
        }

        public async Task<string> UploadBlob(string containerName, IFormFile file)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobName = file.FileName;
            var blobClient = containerClient.GetBlobClient(blobName);

            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            };

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;
            await blobClient.UploadAsync(stream, blobHttpHeaders);
            var blobUri = blobClient.Uri.ToString();

            if (blobUri is not null)
            {
                return blobUri;
            }

            return string.Empty;
        }
    }
}
