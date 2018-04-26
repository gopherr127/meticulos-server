namespace Meticulos.Api.App.WorkflowFunctions
{
    public class WorkflowFunction : Entity
    {
        public WorkflowFunctionTypes Type { get; set; }
        public string Name { get; set; }
    }
}
