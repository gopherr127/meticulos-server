using MongoDB.Bson;

namespace Meticulos.Api.App.ItemLinks
{
    public class ItemLink
    {
        public ObjectId fromItemId { get; set; }
        public ObjectId toItemId { get; set; }
    }
}