using Meticulos.Api.App.Fields;
using Meticulos.Api.App.Items;
using Meticulos.Api.App.ItemTypes;
using Meticulos.Api.App.WorkflowFunctions;
using Meticulos.Api.App.WorkflowFunctions.PostFunctions;
using Meticulos.Api.App.WorkflowFunctions.PreConditions;
using Meticulos.Api.App.WorkflowFunctions.Validations;
using Meticulos.Api.App.WorkflowNodes;
using Meticulos.Api.App.WorkflowTransitionFunctions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Meticulos.Api.App.WorkflowTransitions
{
    [Route("api/[controller]")]
    public class WorkflowTransitionsController : Controller
    {
        private readonly IWorkflowTransitionRepository _workflowTransitionRepository;
        //private readonly IFunctionRegistry _functionRegistry;
        private readonly IItemRepository _itemRepository;
        private readonly IItemTypeRepository _itemTypeRepository;
        private readonly IWorkflowNodeRepository _workflowNodeRepository;
        private readonly IFieldRepository _fieldRepository;

        public WorkflowTransitionsController(
            IWorkflowTransitionRepository workflowTransitionRepository,
            //IFunctionRegistry functionRegistry,
            IItemRepository itemRepository,
            IItemTypeRepository itemTypeRepository,
            IWorkflowNodeRepository workflowNodeRepository,
            IFieldRepository fieldRepository)
        {
            _workflowTransitionRepository = workflowTransitionRepository;
            //_functionRegistry = functionRegistry;
            _itemRepository = itemRepository;
            _itemTypeRepository = itemTypeRepository;
            _workflowNodeRepository = workflowNodeRepository;
            _fieldRepository = fieldRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                return await _workflowTransitionRepository.GetAll();
            });
        }

        [Route("{id:length(24)}")]
        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                return await _workflowTransitionRepository.Get(new ObjectId(id));
            });
        }
        
        [Route("search")]
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery]WorkflowTransitionSearchRequest requestArgs)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                if (string.IsNullOrEmpty(requestArgs.WorkflowId) && string.IsNullOrEmpty(requestArgs.FromNodeId))
                {
                    throw new System.Exception("Valid search criteria not supplied.");
                }

                return await _workflowTransitionRepository.Search(requestArgs);
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]WorkflowTransition item)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                return await _workflowTransitionRepository.Add(item);
            });
        }

        [Route("executions/{id:length(24)}")]
        [HttpPost]
        public async Task<IActionResult> ExecuteTransition(string id, [FromBody]WorkflowTransitionExecutionRequest requestArgs)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {
                // This method assumes that a client has recognized the need to display screens
                // in order to populate all required fields or make any field updates to the item in transition

                if (string.IsNullOrEmpty(requestArgs.ItemId))
                {
                    throw new Exception("Required fields not supplied.");
                }

                Item item = null;
                string serializedObject = "";

                item = await _itemRepository.Get(new ObjectId(requestArgs.ItemId), null);

                if (item == null)
                    throw new Exception("Cannot find item specified.");

                //TODO: Verify that the transition is valid for the item's current state/node

                serializedObject = JsonConvert.SerializeObject(item);
                
                var transition = await _workflowTransitionRepository.Get(new ObjectId(id));

                if (transition == null)
                    throw new Exception("Cannot find transition.");

                FunctionRegistry functionRegistry = new FunctionRegistry(
                    _itemRepository, _itemTypeRepository, _fieldRepository, _workflowNodeRepository);
                WorkflowTransitionExecutionResult result = new WorkflowTransitionExecutionResult();

                if (transition.PreConditions != null)
                {   // Execute all pre-conditions for the transition
                    foreach (WorkflowTransitionFunction funcRef in transition.PreConditions)
                    {
                        var execResult = await functionRegistry.ExecuteFunction(funcRef, item);

                        if (execResult.IsFailure)
                            result.ErrorMessages.Add(execResult.DisplayMessage);
                    }
                }

                // Stop now if any conditions fail
                if (result.ErrorMessages.Count > 0)
                    return result;

                if (transition.Validations != null)
                {   // Execute all validations for the transition
                    foreach (WorkflowTransitionFunction funcRef in transition.Validations)
                    {
                        var execResult = await functionRegistry.ExecuteFunction(funcRef, item);

                        if (execResult.IsFailure)
                            result.ErrorMessages.Add(execResult.DisplayMessage);
                    }
                }

                // Stop now if any validations fail
                if (result.ErrorMessages.Count > 0)
                    return result;

                if (transition.PostFunctions != null)
                {
                    foreach (WorkflowTransitionFunction funcRef in transition.PostFunctions)
                    {
                        var execResult = await functionRegistry.ExecuteFunction(funcRef, item);

                        if (execResult.IsFailure)
                            throw new ApplicationException(execResult.DisplayMessage);
                    }
                }

                // Set Item Workflow Node / Status (should this be optional to avoid having
                // transitions that simply return to the same status just to execute functions?)
                WorkflowNode node = await _workflowNodeRepository.Get(transition.ToNodeId);
                if (node == null)
                    throw new Exception("Cannot find destination node.");
                
                item.WorkflowNodeId = node.Id;
                await _itemRepository.Update(item);

                if (result.ErrorMessages.Count > 0)
                    return result;

                return result;
            });
        }

        [Route("{id:length(24)}")]
        [HttpPut]
        public async Task<IActionResult> Put(string id, [FromBody]WorkflowTransition item)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                item.Id = new ObjectId(id);
                return await _workflowTransitionRepository.Update(item);
            });
        }

        [Route("{id:length(24)}")]
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            return await FunctionWrapper.ExecuteAction(this, async () =>
            {

                await _workflowTransitionRepository.Delete(new ObjectId(id));
            });
        }
    }
}
