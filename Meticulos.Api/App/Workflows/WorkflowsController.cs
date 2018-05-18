using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace Meticulos.Api.App.Workflows
{
    [Route("api/[controller]")]
    public class WorkflowsController : Controller
    {
        private readonly IWorkflowRepository _workflowRepository;

        public WorkflowsController(IWorkflowRepository workflowRepository)
        {
            _workflowRepository = workflowRepository;
        }
        
        [Route("{id:length(24)}")]
        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {
                return await _workflowRepository.Get(new ObjectId(id));
            });
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {
                return await _workflowRepository.GetAll();
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Workflow item)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {
                return await _workflowRepository.Add(item);
            });
        }

        [Route("{id:length(24)}")]
        [HttpPut]
        public async Task<IActionResult> Put(string id, [FromBody]Workflow item)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {
                item.Id = new ObjectId(id);
                return await _workflowRepository.Update(item);
            });
        }

        [Route("{id:length(24)}")]
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            return await FunctionWrapper.ExecuteAction(this, async () =>
            {
                await _workflowRepository.Delete(new ObjectId(id));
            });
        }
    }
}
