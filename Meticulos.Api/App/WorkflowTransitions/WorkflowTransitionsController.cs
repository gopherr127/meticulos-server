using Meticulos.Api.App.Fields;
using Meticulos.Api.App.Items;
using Meticulos.Api.App.WorkflowFunctions.PostFunctions;
using Meticulos.Api.App.WorkflowFunctions.PreConditions;
using Meticulos.Api.App.WorkflowFunctions.Validations;
using Meticulos.Api.App.WorkflowNodes;
using Meticulos.Api.App.WorkflowTransitionFunctions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Meticulos.Api.App.WorkflowTransitions
{
    [Route("api/[controller]")]
    public class WorkflowTransitionsController : Controller
    {
        private readonly IWorkflowTransitionRepository _workflowTransitionRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IWorkflowNodeRepository _workflowNodeRepository;
        private readonly IFieldRepository _fieldRepository;

        public WorkflowTransitionsController(
            IWorkflowTransitionRepository workflowTransitionRepository,
            IItemRepository itemRepository,
            IWorkflowNodeRepository workflowNodeRepository,
            IFieldRepository fieldRepository)
        {
            _workflowTransitionRepository = workflowTransitionRepository;
            _itemRepository = itemRepository;
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
                    throw new System.Exception("Required fields not supplied.");
                }

                Item item = null;
                string serializedObject = "";

                item = await _itemRepository.Get(new ObjectId(requestArgs.ItemId));
                serializedObject = JsonConvert.SerializeObject(item);
                
                if (item == null)
                    throw new System.Exception("Cannot find item specified.");

                var transition = await _workflowTransitionRepository.Get(new ObjectId(id));

                if (transition == null)
                    throw new System.Exception("Cannot find transition.");

                WorkflowTransitionExecutionResult result = new WorkflowTransitionExecutionResult();

                if (transition.PreConditions != null)
                {
                    foreach (WorkflowTransitionFunction funcRef in transition.PreConditions)
                    {
                        switch (funcRef.FunctionId.ToString())
                        {   //TODO: Extract this mapping to a config file
                            case "5aa805dd0af6814a103b25ad":
                                {
                                    var execResult = await new UserInRolePreCondition().Execute(funcRef.FunctionArgs);
                                    if (execResult.IsFailure)
                                        result.ErrorMessages.Add(execResult.DisplayMessage);
                                    break;
                                }
                            case "5aa805dd0af6814a103b25ae":
                                {
                                    var execResult = await new UserInGroupPreCondition().Execute(funcRef.FunctionArgs);
                                    if (execResult.IsFailure)
                                        result.ErrorMessages.Add(execResult.DisplayMessage);
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                    }
                }

                if (result.ErrorMessages.Count > 0)
                    return result;

                if (transition.Validations != null)
                {
                    foreach (WorkflowTransitionFunction funcRef in transition.Validations)
                    {
                        switch (funcRef.FunctionId.ToString())
                        {
                            case "5aa805de0af6814a103b25b0":
                                {
                                    var functionArgs = JsonConvert
                                        .DeserializeObject<FieldRequiredValidationArgs>(funcRef.FunctionArgs);
                                    
                                    var execResult = await new FieldRequiredValidation(_fieldRepository, item)
                                        .Execute(JsonConvert.SerializeObject(functionArgs));

                                    if (execResult.IsFailure)
                                        result.ErrorMessages.Add(execResult.DisplayMessage);
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                    }
                }

                if (result.ErrorMessages.Count > 0)
                    return result;

                if (transition.PostFunctions != null)
                {
                    foreach (WorkflowTransitionFunction funcRef in transition.PostFunctions)
                    {
                        switch (funcRef.FunctionId.ToString())
                        {
                            case "5aa805e00af6814a103b25b5":
                                {   // Make API call
                                    var functionArgs = JsonConvert.DeserializeObject<MakeApiCallPostFunctionArgs>(funcRef.FunctionArgs);

                                    if (functionArgs.IncludePayload)
                                    {   // Inject the item undergoing transition into the payload for this function
                                        functionArgs.Payload = serializedObject;
                                    }

                                    var execResult = await new MakeApiCallPostFunction()
                                        .Execute(JsonConvert.SerializeObject(functionArgs));

                                    //TODO: Add display message to failure notification, not validation results
                                    // since we're past the point of validating something...this is just an error.
                                    if (execResult.IsFailure)
                                        result.ErrorMessages.Add(execResult.DisplayMessage);
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                    }
                }

                // Set Item Workflow Node / Status (should this be optional to avoid having
                // transitions that simply return to the same status just to execute functions?)
                WorkflowNode node = await _workflowNodeRepository.Get(transition.ToNodeId);
                if (node == null)
                    throw new System.Exception("Cannot find destination node.");
                
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
