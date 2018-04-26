using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.WorkflowTransitionFunctions
{
    [Route("api/[controller]")]
    public class WorkflowTransitionFunctionsController : Controller
    {
        private readonly IWorkflowTransitionFunctionRepository _workflowTransitionFunctionRepository;

        public WorkflowTransitionFunctionsController(
            IWorkflowTransitionFunctionRepository workflowTransitionFunctionRepository)
        {
            _workflowTransitionFunctionRepository = workflowTransitionFunctionRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                return await _workflowTransitionFunctionRepository.GetAll();
            });
        }

        [Route("{id:length(24)}")]
        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                return await _workflowTransitionFunctionRepository.Get(new ObjectId(id));
            });
        }

        [Route("search")]
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery]WorkflowTransitionFunctionSearchRequest requestArgs)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                if (string.IsNullOrEmpty(requestArgs.TransitionId))
                {
                    throw new ApplicationException("Search parameters missing.");
                }

                return await _workflowTransitionFunctionRepository.Search(requestArgs);
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]WorkflowTransitionFunctionPostRequest request)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                if (request.TransitionId == ObjectId.Empty)
                    throw new ApplicationException("TransitionId is required.");

                if (request.FunctionId == ObjectId.Empty)
                    throw new ApplicationException("FunctionId is required.");

                var transFunction = new WorkflowTransitionFunction()
                {
                    TransitionId = request.TransitionId,
                    FunctionId = request.FunctionId,
                    FunctionArgs = request.FunctionArgs
                };
                return await _workflowTransitionFunctionRepository.Add(transFunction);
            });
        }

        [Route("{id:length(24)}")]
        [HttpPut]
        public async Task<IActionResult> Put(string id, [FromBody]WorkflowTransitionFunction item)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                item.Id = new ObjectId(id);
                return await _workflowTransitionFunctionRepository.Update(item);
            });
        }

        [Route("{id:length(24)}")]
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            return await FunctionWrapper.ExecuteAction(this, async () =>
            {

                await _workflowTransitionFunctionRepository.Delete(new ObjectId(id));
            });
        }
    }
}