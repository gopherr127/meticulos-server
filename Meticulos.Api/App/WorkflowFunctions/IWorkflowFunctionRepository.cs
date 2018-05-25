using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.WorkflowFunctions
{
    public interface IWorkflowFunctionRepository
    {
        Task<WorkflowFunction> Get(ObjectId id);
        Task<IEnumerable<WorkflowFunction>> Search(WorkflowFunctionSearchRequest requestArgs);
        Task Add(WorkflowFunction item);
    }
}