using MongoDB.Bson;

namespace Meticulos.Api.App.WorkflowNodes
{
    public class WorkflowNode : Entity
    {
        public string Name { get; set; }
        public ObjectId WorkflowId { get; set; }
    }
}
