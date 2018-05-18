using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Meticulos.Api.App.Workflows
{
    public class WorkflowContext : ContextBase
    {
        public WorkflowContext(IOptions<Settings> settings) : base(settings) { }

        public IMongoCollection<Workflow> Workflows
        {
            get { return DefaultDatabase.GetCollection<Workflow>("Workflows"); }
        }
    }
}
