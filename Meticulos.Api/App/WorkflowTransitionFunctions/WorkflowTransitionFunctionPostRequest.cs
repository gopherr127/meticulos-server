using MongoDB.Bson;

namespace Meticulos.Api.App.WorkflowTransitionFunctions
{
    public class WorkflowTransitionFunctionPostRequest
    {
        public ObjectId FunctionId { get; set; }
        public ObjectId TransitionId { get; set; }
        public string FunctionArgs { get; set; }
    }
}
