using MongoDB.Bson;

namespace Meticulos.Api.App.ItemTypes
{
    public class ItemType : Entity
    {
        public string Name { get; set; }
        public ObjectId WorkflowId { get; set; }
        public string IconUrl { get; set; }
        public bool IsForPysicalItems { get; set; }
        public bool AllowNestedItems { get; set; }

        public ObjectId CreateScreenId { get; set; }
        public ObjectId EditScreenId { get; set; }
        public ObjectId ViewScreenId { get; set; }
    }
}
