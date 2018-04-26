using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Meticulos.Api.App.WorkflowNodes
{
    public class WorkflowNodeContext : ContextBase
    {
        public WorkflowNodeContext(IOptions<Settings> settings) : base(settings) { }

        public IMongoCollection<WorkflowNode> WorkflowNodes
        {
            get { return Database.GetCollection<WorkflowNode>("WorkflowNodes"); }
        }
    }
}
