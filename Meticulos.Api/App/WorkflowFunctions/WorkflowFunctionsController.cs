using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.WorkflowFunctions
{
    [Route("api/WorkflowFunctions")]
    public class WorkflowFunctionsController : Controller
    {
        private readonly IWorkflowFunctionRepository _workflowFunctionRepository;

        public WorkflowFunctionsController(
            IWorkflowFunctionRepository workflowFunctionRepository)
        {
            _workflowFunctionRepository = workflowFunctionRepository;
        }
        
        [Route("search")]
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery]WorkflowFunctionSearchRequest requestArgs)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                if (!Enum.IsDefined(typeof(WorkflowFunctionTypes), requestArgs.Type))
                {
                    throw new ApplicationException("Invalid function type specified.");
                }

                return await _workflowFunctionRepository.Search(requestArgs);
            });
        }

    }
}
