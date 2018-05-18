using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Meticulos.Api.App.WorkflowTransitionFunctions
{
    public class WorkflowTransitionFunctionContext : ContextBase
    {
        public WorkflowTransitionFunctionContext(IOptions<Settings> settings) : base(settings) { }

        public IMongoCollection<WorkflowTransitionFunction> WorkflowTransitionFunctions
        {
            get { return DefaultDatabase.GetCollection<WorkflowTransitionFunction>("WorkflowTransitionFunctions"); }
        }
    }
}
