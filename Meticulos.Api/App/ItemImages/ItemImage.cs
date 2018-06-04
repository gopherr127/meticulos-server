using MongoDB.Bson;

namespace Meticulos.Api.App.ItemImages
{
    public class ItemImage : Entity
    {
        public ObjectId TargetItemId { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
        public string FileMetadata { get; set; }
    }
}
