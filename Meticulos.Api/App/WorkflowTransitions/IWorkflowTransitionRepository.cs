using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.WorkflowTransitions
{
    public interface IWorkflowTransitionRepository
    {
        Task<IEnumerable<WorkflowTransition>> GetAll();
        Task<IEnumerable<WorkflowTransition>> Search(WorkflowTransitionSearchRequest requestArgs);
        Task<WorkflowTransition> Get(ObjectId id);
        Task<WorkflowTransition> Add(WorkflowTransition item);
        Task<WorkflowTransition> Update(WorkflowTransition item);
        Task Delete(ObjectId id);
    }
}