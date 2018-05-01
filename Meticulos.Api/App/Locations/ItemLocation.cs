using MongoDB.Bson;

namespace Meticulos.Api.App.Locations
{
    public class ItemLocation : Entity
    {
        public string Name { get; set; }
        public ObjectId ParentId { get; set; }
        public ItemLocation Parent { get; set; }
        public GpsLocation Gps { get; set; }
    }
}
