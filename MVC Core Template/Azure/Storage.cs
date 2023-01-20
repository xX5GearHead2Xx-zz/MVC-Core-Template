using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Hosting;
using System.Reflection.Metadata;

namespace Ecommerce.Azure
{
    public class Storage
    {
        private static string Connection;
        private static string ContainerName;

        public Storage()
        {
            Connection = Configuration.configuration["AzureStorage_ProductImages:ConnectionString"];
            ContainerName = Configuration.configuration["AzureStorage_ProductImages:Container"];
        }

        public async Task Upload(string FileKey, byte[] Data)
        {
            try
            {
                var blobClient = new BlobContainerClient(Connection, ContainerName);
                var blob = blobClient.GetBlobClient(FileKey);
                if (blob.Exists())
                {
                    await blob.DeleteAsync();
                }
                Stream stream = new MemoryStream(Data);
                await blob.UploadAsync(stream);
            }
            catch (Exception Ex)
            {
                throw new Exception("Azure > Storage > UploadImage " + Ex.Message);
            }
        }

        public async Task Delete(string FileKey)
        {
            try
            {
                var blobClient = new BlobContainerClient(Connection, ContainerName);
                var blob = blobClient.GetBlobClient(FileKey);
                await blob.DeleteIfExistsAsync();
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > AzureStorage >  DeleteImage" + Ex.Message);
            }
        }

        public async Task<byte[]> Download(string FileKey)
        {
            try
            {
                var blobClient = new BlobContainerClient(Connection, ContainerName);
                var blob = blobClient.GetBlobClient(FileKey);
                using (var ms = new MemoryStream())
                {
                    BlobDownloadResult Result = await blob.DownloadContentAsync();
                    return Result.Content.ToArray();
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > AzureStorage > UploadImage " + Ex.Message);
            }
        }
    }
}
