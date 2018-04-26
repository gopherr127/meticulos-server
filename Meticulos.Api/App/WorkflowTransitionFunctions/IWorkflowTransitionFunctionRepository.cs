using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.WorkflowTransitionFunctions
{
    public interface IWorkflowTransitionFunctionRepository
    {
        Task<IEnumerable<WorkflowTransitionFunction>> GetAll();
        Task<IEnumerable<WorkflowTransitionFunction>> Search(WorkflowTransitionFunctionSearchRequest requestArgs);
        Task<WorkflowTransitionFunction> Get(ObjectId id);
        Task<WorkflowTransitionFunction> Add(WorkflowTransitionFunction item);
        Task<WorkflowTransitionFunction> Update(WorkflowTransitionFunction item);
        Task Delete(ObjectId id);
    }
}