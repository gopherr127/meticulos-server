using Meticulos.Api.App.WorkflowFunctions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Meticulos.Api.App.WorkflowTransitionFunctions
{
    public class WorkflowTransitionFunction : Entity
    {
        public ObjectId FunctionId { get; set; }
        public ObjectId TransitionId { get; set; }
        public string FunctionArgs { get; set; }

        [BsonIgnore]
        public WorkflowFunction Function { get; set; }
    }
}
