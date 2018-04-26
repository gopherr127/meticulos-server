using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.WorkflowNodes
{
    public interface IWorkflowNodeRepository
    {
        Task<IEnumerable<WorkflowNode>> GetAll();
        Task<IEnumerable<WorkflowNode>> Search(ObjectId workflowId);
        Task<WorkflowNode> Get(ObjectId id);
        Task<WorkflowNode> Add(WorkflowNode item);
        Task<WorkflowNode> Update(WorkflowNode item);
        Task Delete(ObjectId id);
    }
}