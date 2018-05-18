using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Meticulos.Api.App.WorkflowTransitions
{
    public class WorkflowTransitionContext : ContextBase
    {
        public WorkflowTransitionContext(IOptions<Settings> settings) : base(settings) { }

        public IMongoCollection<WorkflowTransition> WorkflowTransitions
        {
            get { return DefaultDatabase.GetCollection<WorkflowTransition>("WorkflowTransitions"); }
        }
    }
}
