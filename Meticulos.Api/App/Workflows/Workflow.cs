using System.Collections.Generic;
using Meticulos.Api.App.WorkflowTransitions;
using MongoDB.Bson;

namespace Meticulos.Api.App.Workflows
{
    public class Workflow : Entity
    {
        public string Name { get; set; }
        public List<ObjectId> Nodes { get; set; }
        public List<WorkflowTransition> Transitions { get; set; }
    }
}
