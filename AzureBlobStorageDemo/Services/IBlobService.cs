using AzureBlobStorageDemo.Models;

namespace AzureBlobStorageDemo.Services
{
    public interface IBlobService
    {
        Task<List<BlobModel>> GetAllBlobs(string containerName);
        Task<BlobModel> GetBlob(string containerName, string blobName);
        Task<string> UploadBlob(string containerName, IFormFile file);
        Task<bool> DeleteBlob(string containerName, string blobName);
        Task<FormFile?> DownloadBlob(string containerName, string blobName);
    }
}
