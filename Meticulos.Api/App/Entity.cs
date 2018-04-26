using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Meticulos.Api.App
{
    public class Entity
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public Entity()
        {
            Id = ObjectId.GenerateNewId();
        }
    }
}
