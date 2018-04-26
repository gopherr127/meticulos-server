using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.Workflows
{
    public interface IWorkflowRepository
    {
        Task<IEnumerable<Workflow>> GetAll();
        Task<Workflow> Get(ObjectId id);
        Task<Workflow> Add(Workflow item);
        Task<Workflow> Update(Workflow item);
        Task Delete(ObjectId id);
    }
}