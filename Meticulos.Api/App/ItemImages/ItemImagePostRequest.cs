namespace Meticulos.Api.App.ItemImages
{
    public class ItemImagePostRequest
    {
        public string TargetItemId { get; set; }
        public string FileName { get; set; }
        public string FileMetadata { get; set; }
        public string ImageData { get; set; }
    }
}