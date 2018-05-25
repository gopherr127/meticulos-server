using Meticulos.Api.App.Fields;
using Meticulos.Api.App.Items;
using Meticulos.Api.App.ItemTypes;
using Meticulos.Api.App.WorkflowFunctions.PostFunctions;
using Meticulos.Api.App.WorkflowFunctions.PreConditions;
using Meticulos.Api.App.WorkflowFunctions.Validations;
using Meticulos.Api.App.WorkflowNodes;
using Meticulos.Api.App.WorkflowTransitionFunctions;
using MongoDB.Bson;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.WorkflowFunctions
{
    public class FunctionRegistry : IFunctionRegistry
    {
        private readonly IItemRepository _itemRepository;
        private readonly IItemTypeRepository _itemTypeRepository;
        private readonly IFieldRepository _fieldRepository;
        private readonly IWorkflowNodeRepository _workflowNodeRepository;

        List<WorkflowFunction> defaultFunctions = new List<WorkflowFunction>();
        // Pre-Conditions
        private const string UserInRolePreConditionId               = "5aa805dd0af6814a103b25ad";
        private const string UserInGroupPreConditionId              = "5aa805dd0af6814a103b25ae";
        private const string FieldValueComparisonPreConditionId     = "5aa805de0af6814a103b25af";
        private const string LinkedItemOfTypeInStatusPreConditionId = "5b061754fc312607140abb54";
        // Validations
        private const string FieldValueRequiredValidationId         = "5aa805de0af6814a103b25b0";
        private const string FieldValueComparisonValidationId       = "5aa805df0af6814a103b25b1";
        // Post-Functions
        private const string SetFieldValuePostFunctionId            = "5aa805df0af6814a103b25b2";
        private const string SendEmailPostFunctionId                = "5aa805e00af6814a103b25b3";
        private const string MakeApiCallPostFunctionId              = "5aa805e00af6814a103b25b5";

        public FunctionRegistry(
            IItemRepository itemRepository,
            IItemTypeRepository itemTypeRepository,
            IFieldRepository fieldRepository,
            IWorkflowNodeRepository workflowNodeRepository)
        {
            _itemRepository = itemRepository;
            _itemTypeRepository = itemTypeRepository;
            _fieldRepository = fieldRepository;
            _workflowNodeRepository = workflowNodeRepository;
        }

        public List<WorkflowFunction> GetDefaultFunctions()
        {
            defaultFunctions.Add(new WorkflowFunction()
            {
                Id = ObjectId.Parse(UserInRolePreConditionId),
                Type = WorkflowFunctionTypes.PreCondition,
                Name = "User is in Role"
            });
            defaultFunctions.Add(new WorkflowFunction()
            {
                Id = ObjectId.Parse(UserInGroupPreConditionId),
                Type = WorkflowFunctionTypes.PreCondition,
                Name = "User is in Group"
            });
            defaultFunctions.Add(new WorkflowFunction()
            {
                Id = ObjectId.Parse(FieldValueComparisonPreConditionId),
                Type = WorkflowFunctionTypes.PreCondition,
                Name = "Field value comparison"
            });
            defaultFunctions.Add(new WorkflowFunction()
            {
                Id = ObjectId.Parse(LinkedItemOfTypeInStatusPreConditionId),
                Type = WorkflowFunctionTypes.PreCondition,
                Name = "Linked Item of Type in Status"
            });

            // Validations
            defaultFunctions.Add(new WorkflowFunction()
            {
                Id = ObjectId.Parse(FieldValueRequiredValidationId),
                Type = WorkflowFunctionTypes.Validation,
                Name = "Field value required"
            });
            defaultFunctions.Add(new WorkflowFunction()
            {
                Id = ObjectId.Parse(FieldValueComparisonValidationId),
                Type = WorkflowFunctionTypes.Validation,
                Name = "Field value comparison"
            });

            // Post-Functions
            defaultFunctions.Add(new WorkflowFunction()
            {
                Id = ObjectId.Parse(SetFieldValuePostFunctionId),
                Type = WorkflowFunctionTypes.PostFunction,
                Name = "Set field value"
            });
            defaultFunctions.Add(new WorkflowFunction()
            {
                Id = ObjectId.Parse(SendEmailPostFunctionId),
                Type = WorkflowFunctionTypes.PostFunction,
                Name = "Send email notification"
            });
            defaultFunctions.Add(new WorkflowFunction()
            {
                Id = ObjectId.Parse(MakeApiCallPostFunctionId),
                Type = WorkflowFunctionTypes.PostFunction,
                Name = "Make API call"
            });

            return defaultFunctions;
        }

        public Task<OperationResult<bool>> ExecuteFunction(
            WorkflowTransitionFunction transitionFunction,
            Item item = null)
        {
            return Task.Run( async () =>
            {
                switch (transitionFunction.FunctionId.ToString())
                {
                    case UserInRolePreConditionId:
                        {
                            return await new UserInRolePreCondition()
                                .Execute(transitionFunction.FunctionArgs);
                        }
                    case UserInGroupPreConditionId:
                        {
                            return await new UserInGroupPreCondition()
                                .Execute(transitionFunction.FunctionArgs);
                        }
                    case LinkedItemOfTypeInStatusPreConditionId:
                        {
                            return await new LinkedItemOfTypeInStatusPreCondition(
                                item,
                                _itemTypeRepository, _workflowNodeRepository)
                                .Execute(transitionFunction.FunctionArgs);
                        }
                    case FieldValueRequiredValidationId:
                        {
                            return await new FieldRequiredValidation(
                                _fieldRepository,
                                item)
                                .Execute(transitionFunction.FunctionArgs);
                        }
                    case MakeApiCallPostFunctionId:
                        {
                            var functionArgs = JsonConvert
                                .DeserializeObject<MakeApiCallPostFunctionArgs>(
                                    transitionFunction.FunctionArgs);

                            if (functionArgs.IncludePayload)
                            {
                                functionArgs.Payload = JsonConvert.SerializeObject(item);
                            }

                            return await new MakeApiCallPostFunction()
                                .Execute(JsonConvert.SerializeObject(functionArgs));
                        }
                    case SetFieldValuePostFunctionId:
                        {
                            return await new SetFieldValuePostFunction(
                                _fieldRepository,
                                _itemRepository,
                                item)
                                .Execute(transitionFunction.FunctionArgs);
                        }
                    case SendEmailPostFunctionId:
                        {
                            return await new SendEmailPostFunction(item)
                                .Execute(transitionFunction.FunctionArgs);
                        }
                    default:
                        {
                            break;
                        }
                }

                return new OperationResult<bool>() { Value = true };
            });


        }
    }
}
