using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.IO;

namespace StackOverflowService.Helpers
{
    public class BlobHelper
    {
        public static string CreateBlobImage(string rowKey, HttpPostedFileBase profileImage)
        {
			try
			{
                // kreiranje blob sadrzaja i kreiranje blob klijenta
                string uniqueBlobName = string.Format("image_{0}_{1}", rowKey, DateTime.UtcNow.Ticks);
                var storageAccount =
                CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
                CloudBlobClient blobStorage = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobStorage.GetContainerReference("vezba");
                CloudBlockBlob blob = container.GetBlockBlobReference(uniqueBlobName);
                blob.Properties.ContentType = profileImage.ContentType;
                // postavljanje odabrane datoteke (slike) u blob servis koristeci blob klijent
                blob.UploadFromStream(profileImage.InputStream);

                return blob.Uri.ToString();
            }
			catch
			{
                return null;
			}
        }

        public static bool DeleteBlobImage(string pictureUrl)
        {
            if (string.IsNullOrEmpty(pictureUrl))
            {
                return false;
            }

            try
            {
                string blobName = Path.GetFileName(new Uri(pictureUrl).LocalPath);
                var storageAccount = CloudStorageAccount.Parse(
                    CloudConfigurationManager.GetSetting("DataConnectionString"));
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("vezba");

                // Brisanje blob-a
                CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
                return blob.DeleteIfExists();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting blob: {ex.Message}");
                return false;
            }
        }
    }
}