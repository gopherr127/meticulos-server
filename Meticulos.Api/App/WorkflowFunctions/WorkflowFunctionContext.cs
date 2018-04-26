using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Meticulos.Api.App.WorkflowFunctions
{
    public class WorkflowFunctionContext : ContextBase
    {
        public WorkflowFunctionContext(IOptions<Settings> settings) : base(settings) { }

        public IMongoCollection<WorkflowFunction> WorkflowFunctions
        {
            get { return Database.GetCollection<WorkflowFunction>("WorkflowFunctions"); }
        }
    }
}
