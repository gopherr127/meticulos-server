using Meticulos.Api.App.Items;

namespace Meticulos.Api.App.WorkflowTransitions
{
    public class WorkflowTransitionExecutionRequest
    {
        public string ItemId { get; set; }
        public ItemCategory ItemCategory { get; set; }
        public Item ItemData { get; set; }
    }
}
