using Meticulos.Api.App.Workflows;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.WorkflowNodes
{
    [Route("api/[controller]")]
    public class WorkflowNodesController : Controller
    {
        private readonly IWorkflowNodeRepository _workflowNodeRepository;
        private readonly IWorkflowRepository _workflowRepository;

        public WorkflowNodesController(IWorkflowNodeRepository workflowNodeRepository,
            IWorkflowRepository workflowRepository)
        {
            _workflowNodeRepository = workflowNodeRepository;
            _workflowRepository = workflowRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                return await _workflowNodeRepository.GetAll();
            });
        }

        [Route("{id:length(24)}")]
        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                return await _workflowNodeRepository.Get(new ObjectId(id));
            });
        }

        [Route("search")]
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery]WorkflowNodeSearchRequest requestArgs)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                if (string.IsNullOrEmpty(requestArgs.WorkflowId))
                {
                    throw new System.Exception("Invalid search parameters supplied.");
                }

                return await _workflowNodeRepository.Search(new ObjectId(requestArgs.WorkflowId));
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]WorkflowNode item)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                var newNode = await _workflowNodeRepository.Add(item);
                // Update workflow's Nodes list
                var wf = await _workflowRepository.Get(item.WorkflowId);
                if (wf.Nodes == null)
                    wf.Nodes = new List<ObjectId>();
                wf.Nodes.Add(item.Id);
                await _workflowRepository.Update(wf);

                return newNode;
            });
        }

        [Route("{id:length(24)}")]
        [HttpPut]
        public async Task<IActionResult> Put(string id, [FromBody]WorkflowNode item)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                item.Id = new ObjectId(id);
                return await _workflowNodeRepository.Update(item);
            });
        }

        [Route("{id:length(24)}")]
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            return await FunctionWrapper.ExecuteAction(this, async () =>
            {

                await _workflowNodeRepository.Delete(new ObjectId(id));
            });
        }
    }
}
