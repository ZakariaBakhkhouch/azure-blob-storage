using AzureBlobStorageDemo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace AzureBlobStorageDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobsController : ControllerBase
    {
        private readonly IBlobService _blobService;
        private readonly string containerName = "";

        public BlobsController(IBlobService blobService)
        {
            _blobService = blobService;
        }

        [HttpGet]
        [Route("GetBlobs")]
        public async Task<IActionResult> GetBlobs()
        {
            var blobs = await _blobService.GetAllBlobs(containerName);

            return Ok(blobs);   
        }
        

        [HttpGet]
        [Route("GetBlob/{fileName}")]
        public async Task<IActionResult> GetBlobs(string fileName)
        {
            var blob = await _blobService.GetBlob(containerName, fileName);

            return Ok(blob);   
        }
        
        [HttpPost]
        [Route("UploadBlob")]
        public async Task<IActionResult> UploadBlob(IFormFile file)
        {
            var uri = await _blobService.UploadBlob(containerName, file);

            return Ok(uri);   
        }
        
        [HttpDelete]
        [Route("DeletBlob/{fileName}")]
        public async Task<IActionResult> UploadBlob(string fileName)
        {
            var result = await _blobService.DeleteBlob(containerName, fileName);

            if(result)
                return Ok("Blob deleted");

            return Ok("Error");   
        }
        
        [HttpPost]
        [Route("DownloadBlob/{fileName}")]
        public async Task<IActionResult> Download(string fileName)
        {
            var file = await _blobService.DownloadBlob(containerName, fileName);

            if(file is not null)
            {
                return File(file.OpenReadStream(), "application/octet-stream", fileName);
            }

            return Ok("blob not found");
        }
    }
}
