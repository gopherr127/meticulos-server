namespace Meticulos.Api.App.WorkflowTransitions
{
    public class WorkflowTransitionSearchRequest
    {
        public string WorkflowId { get; set; }
        public string FromNodeId { get; set; }
    }
}
