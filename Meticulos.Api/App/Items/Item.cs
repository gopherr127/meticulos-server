using System.Collections.Generic;
using Meticulos.Api.App.Fields;
using Meticulos.Api.App.ItemTypes;
using Meticulos.Api.App.Locations;
using Meticulos.Api.App.WorkflowNodes;
using MongoDB.Bson;

namespace Meticulos.Api.App.Items
{
    public class Item : Entity
    {
        public ObjectId LocationId { get; set; }
        public ItemLocation Location { get; set; }
        //public List<ObjectId> AncestorIds { get; set; }
        //public ObjectId AssetTypeFilterId { get; set; }
        public List<ObjectId> LinkedItemIds { get; set; }
        public List<Item> LinkedItems { get; set; }
        //public List<ObjectId> AllowedRoles { get; set; }
        public string Name { get; set; }
        public ObjectId TypeId { get; set; }
        public ItemType Type { get; set; }
        //public ObjectId ParentId { get; set; }
        public ObjectId WorkflowNodeId { get; set; }
        public WorkflowNode WorkflowNode { get; set; }
        public List<FieldValue> FieldValues { get; set; }
    }
}
