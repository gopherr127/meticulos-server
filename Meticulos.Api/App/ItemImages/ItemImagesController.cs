using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using MongoDB.Bson;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Meticulos.Api.App.ItemImages
{
    [Route("api/[controller]")]
    public class ItemImagesController : Controller
    {
        private StorageCredentials GetStorageCredentials()
        {
            return new StorageCredentials(
                    "meticulos",
                    "j020NBCxHEVveapl/86LrxkeYpuecAknpI9nGn3KMU+rxP8E28X6g5bG1TE2TGJcDAslfrV0asFfar8TGso8Wg==");
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]string imageFileName)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {
                if (string.IsNullOrEmpty(imageFileName))
                    return "";

                var url = string.Concat(
                    "https://meticulos.blob.core.windows.net/images/",
                    imageFileName);

                var blob = new CloudBlockBlob(new Uri(url), GetStorageCredentials());
                if (await blob.ExistsAsync())
                {

                    await blob.FetchAttributesAsync();
                    byte[] imageBytes = new byte[blob.Properties.Length];
                    await blob.DownloadToByteArrayAsync(imageBytes, 0);
                    return Convert.ToBase64String(imageBytes);
                }

                return "";
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ItemImagePostRequest request)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () => {
                
                byte[] imgBytes = Convert.FromBase64String(request.ImageData);

                var url = string.Concat(
                    "https://meticulos.blob.core.windows.net/images/",
                    request.FileName);

                var blob = new CloudBlockBlob(new Uri(url), GetStorageCredentials());

                if (!(await blob.ExistsAsync()))
                {
                    await blob.UploadFromByteArrayAsync(imgBytes, 0, imgBytes.Length);
                }

                return new ItemImage()
                {
                    Url = url
                };
            });
        }

    }
}
