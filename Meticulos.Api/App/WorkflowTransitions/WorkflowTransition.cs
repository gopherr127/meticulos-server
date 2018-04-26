using System.Collections.Generic;
using Meticulos.Api.App.Screens;
using Meticulos.Api.App.WorkflowNodes;
using Meticulos.Api.App.WorkflowTransitionFunctions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Meticulos.Api.App.WorkflowTransitions
{
    public class WorkflowTransition : Entity
    {
        public string Name { get; set; }
        public ObjectId WorkflowId { get; set; }
        public ObjectId FromNodeId { get; set; }
        public WorkflowNode FromNode { get; set; }
        public ObjectId ToNodeId { get; set; }
        public WorkflowNode ToNode { get; set; }
        public List<ObjectId> ScreenIds { get; set; }

        // For hydrating on GET only
        [BsonIgnore]
        public List<Screen> Screens { get; set; }
        [BsonIgnore]
        public List<WorkflowTransitionFunction> PreConditions { get; set; }
        [BsonIgnore]
        public List<WorkflowTransitionFunction> Validations { get; set; }
        [BsonIgnore]
        public List<WorkflowTransitionFunction> PostFunctions { get; set; }
    }
}
