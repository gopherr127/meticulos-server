using Meticulos.Api.App.Items;
using Meticulos.Api.App.ItemTypes;
using Meticulos.Api.App.WorkflowNodes;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Meticulos.Api.App.WorkflowFunctions.PreConditions
{
    public class LinkedItemOfTypeInStatusPreCondition : WorkflowFunction, IWorkflowFunction
    {
        private readonly IItemTypeRepository _itemTypeRepository;
        private readonly IWorkflowNodeRepository _workflowNodeRepository;
        private readonly Item _item;

        public LinkedItemOfTypeInStatusPreCondition(Item item,
            IItemTypeRepository itemTypeRepository,
            IWorkflowNodeRepository workflowNodeRepository)
        {
            _item = item;
            _itemTypeRepository = itemTypeRepository;
            _workflowNodeRepository = workflowNodeRepository;
        }

        public Task<OperationResult<bool>> Execute(string argsObject)
        {
            return Task.Run( async () =>
            {
                if (_item == null)
                {
                    return new OperationResult<bool>()
                    {
                        Value = false,
                        ErrorMessage = "Item is required."
                    };
                }

                LinkedItemOfTypeInStatusArgs args =
                    JsonConvert.DeserializeObject<LinkedItemOfTypeInStatusArgs>(argsObject);

                if (string.IsNullOrEmpty(args.SelectedItemTypeId) ||
                    string.IsNullOrEmpty(args.SelectedWorkflowNodeId))
                {
                    return new OperationResult<bool>()
                    {
                        Value = false,
                        ErrorMessage = "Post-function is not configured correctly."
                    };
                }

                if (_item.LinkedItems == null || _item.LinkedItems.Count == 0)
                {
                    return new OperationResult<bool>()
                    {
                        Value = false,
                        ErrorMessage = $"No linked item was found."
                    };
                }

                // Check for linked item of appropriate type
                foreach (Item linkedItem in _item.LinkedItems)
                {
                    if (linkedItem.Type.Id == new ObjectId(args.SelectedItemTypeId))
                    {
                        // Check for appropriate status on linked item
                        if (args.SelectedWorkflowNodeId == "000000000000000000000000"
                            || (linkedItem.WorkflowNode.Id == new ObjectId(args.SelectedWorkflowNodeId)))
                        {
                            return new OperationResult<bool>() { Value = true };
                        }
                    }
                }

                var itemType = await _itemTypeRepository.Get(new ObjectId(args.SelectedItemTypeId));
                var workflowNode = await _workflowNodeRepository.Get(new ObjectId(args.SelectedWorkflowNodeId));

                return new OperationResult<bool>()
                {
                    Value = false,
                    ErrorMessage = workflowNode != null
                        ? $"No linked item of type {itemType.Name} with status {workflowNode.Name} was found."
                        : $"No linked item of type {itemType.Name} was found."
                };
            });
        }
    }

    public class LinkedItemOfTypeInStatusArgs
    {
        public string SelectedItemTypeId { get; set; }
        public string SelectedWorkflowNodeId { get; set; }
    }
}
