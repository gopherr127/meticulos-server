using MongoDB.Bson;

namespace Meticulos.Api.App.Locations
{
    public class ItemLocation
    {
        public string Name { get; set; }
        public ObjectId ParentId { get; set; }
        public GpsLocation Gps { get; set; }
    }
}
