using Meticulos.Api.App.ChangeHistory;
using Meticulos.Api.App.Fields;
using Meticulos.Api.App.ItemTypes;
using Meticulos.Api.App.Locations;
using Meticulos.Api.App.WorkflowFunctions;
using Meticulos.Api.App.WorkflowNodes;
using Meticulos.Api.App.WorkflowTransitionFunctions;
using Meticulos.Api.App.WorkflowTransitions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meticulos.Api.App.Items
{
    public class ItemRepository : IItemRepository
    {
        private readonly ItemContext _context = null;
        private readonly IItemTypeRepository _itemTypeRepository;
        private readonly IWorkflowNodeRepository _workflowNodeRepository;
        //private readonly IFieldRepository _fieldRepository;
        private readonly IFieldChangeGroupRepository _fieldChangeGroupRepository;
        private readonly IItemLocationRepository _itemLocationRepository;
        private readonly IWorkflowTransitionRepository _workflowTransitionRepository;
        //private readonly IFunctionRegistry _functionRegistry; <-- causes circular dependency
        private Dictionary<string, ItemType> tempItemTypeCache;
        private Dictionary<string, WorkflowNode> tempWorkflorNodeCache;
        private Dictionary<string, ItemLocation> tempItemLocationCache;

        public ItemRepository(IOptions<Settings> settings,
            IItemTypeRepository itemTypeRepository,
            IWorkflowNodeRepository workflowNodeRepository,
            //IFieldRepository fieldRepository,
            IFieldChangeGroupRepository fieldChangeGroupRepository,
            IItemLocationRepository itemLocationRepository,
            IWorkflowTransitionRepository workflowTransitionRepository)
        {
            _context = new ItemContext(settings);
            _itemTypeRepository = itemTypeRepository;
            _workflowNodeRepository = workflowNodeRepository;
            //_fieldRepository = fieldRepository;
            _fieldChangeGroupRepository = fieldChangeGroupRepository;
            _itemLocationRepository = itemLocationRepository;
            _workflowTransitionRepository = workflowTransitionRepository;
        }
        
        private void ResetTemporaryCaches()
        {
            tempItemTypeCache = new Dictionary<string, ItemType>();
            tempWorkflorNodeCache = new Dictionary<string, WorkflowNode>();
            tempItemLocationCache = new Dictionary<string, ItemLocation>();
        }

        private async Task<Item> HydrateForGetAndSave(Item item)
        {
            try
            {
                // Clear linked items as they should not be saved
                item.LinkedItems = null;

                // Set Type from ID
                string typeId = item.TypeId.ToString();

                if (!tempItemTypeCache.ContainsKey(typeId))
                {
                    var itemType = await _itemTypeRepository.Get(item.TypeId);
                    if (itemType == null)
                        throw new ApplicationException("Invalid Type ID.");
                    tempItemTypeCache.Add(typeId, itemType);
                }
                item.Type = tempItemTypeCache[typeId];

                if (item.LocationId == null && item.LocationId != ObjectId.Empty)
                {
                    string locationId = item.LocationId.ToString();

                    if (!tempItemLocationCache.ContainsKey(locationId))
                    {
                        var loc = await _itemLocationRepository.Get(item.LocationId);
                        if (loc == null)
                            throw new ApplicationException("Invalid Location ID.");
                        tempItemLocationCache.Add(locationId, loc);
                    }
                    item.Location = tempItemLocationCache[locationId];
                }

                // Populate item's workflow node ID from default node from assoc. item type workflow
                // if not already set (should only be for new items)
                if (item.WorkflowNodeId == null || item.WorkflowNodeId == ObjectId.Empty)
                {
                    if (item.Type.WorkflowId == null || item.Type.WorkflowId == ObjectId.Empty)
                        throw new ApplicationException("Item does not have an associated workflow.");
                    var workflowNodes = await _workflowNodeRepository.Search(item.Type.WorkflowId);
                    var workflowNode = workflowNodes.FirstOrDefault();
                    if (workflowNode == null)
                        throw new ApplicationException("Associated workflow does not have any nodes.");
                    item.WorkflowNodeId = workflowNode.Id;
                }

                // Set Workflow Node from ItemType's associated Workflow
                string workflowNodeId = item.WorkflowNodeId.ToString();
                    
                if (!tempWorkflorNodeCache.ContainsKey(workflowNodeId))
                {
                    var workflowNode = await _workflowNodeRepository.Get(item.WorkflowNodeId);
                    if (workflowNode == null)
                        throw new ApplicationException("Cannot find workflow node.");
                    tempWorkflorNodeCache.Add(workflowNodeId, workflowNode);
                }
                item.WorkflowNode = tempWorkflorNodeCache[workflowNodeId];

                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<Item> HydrateForGet(Item item, ItemExpansionParams expand)
        {
            try
            {
                item = await HydrateForGetAndSave(item);
                
                if (expand.LinkedItems)
                {
                    if (item.LinkedItemIds != null && item.LinkedItemIds.Count > 0)
                    {
                        item.LinkedItems = new List<Item>();
                        foreach (ObjectId linkedItemId in item.LinkedItemIds)
                        {
                            if (item.LinkedItems == null)
                                item.LinkedItems = new List<Item>();
                            item.LinkedItems.Add(await Get(linkedItemId));
                        }
                    }
                }

                if (expand.Transitions)
                {
                    var funcReg = new FunctionRegistry(this,
                        _itemTypeRepository, null, _workflowNodeRepository);

                    var nodeTransitions = await _workflowTransitionRepository
                        .Search(new WorkflowTransitionSearchRequest()
                        {
                            FromNodeId = item.WorkflowNodeId.ToString()
                        });

                    // Test each transitions conditions for applicability to the item
                    foreach (WorkflowTransition transition in nodeTransitions)
                    {
                        bool anyFailedConditions = false;

                        if (transition.PreConditions == null)
                            transition.PreConditions = new List<WorkflowTransitionFunction>();

                        foreach (WorkflowTransitionFunction condition in transition.PreConditions)
                        {
                            var execResult = await funcReg
                                .ExecuteFunction(condition, item);

                            if (execResult.IsFailure)
                            {
                                anyFailedConditions = true;
                                break;
                            }
                        }

                        if (!anyFailedConditions)
                        {
                            if (item.Transitions == null)
                                item.Transitions = new List<WorkflowTransition>();

                            item.Transitions.Add(transition);
                        }
                    }
                }

                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<Item> HydrateForSave(Item item)
        {
            try
            {
                item = await HydrateForGetAndSave(item);

                if (item.ParentId != null && item.ParentId != ObjectId.Empty)
                {   // Hydrate Ancestor ID list
                    var parent = await Get(item.ParentId);
                    if (parent != null)
                    {
                        item.AncestorIds = new List<ObjectId>();
                        if (parent.AncestorIds != null)
                        {
                            item.AncestorIds = parent.AncestorIds;
                        }
                        item.AncestorIds.Add(item.ParentId);
                    }
                }

                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Item>> GetAll()
        {
            try
            {
                return await _context.Items.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Item> Get(ObjectId id, ItemExpansionParams expand = null)
        {
            var filter = Builders<Item>.Filter.Eq("Id", id);

            if (expand == null)
                expand = new ItemExpansionParams(null);

            try
            {
                var item = await _context.Items.Find(filter).FirstOrDefaultAsync();
                ResetTemporaryCaches();
                await HydrateForGet(item, expand);
                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Item>> Search(ItemSearchRequest request)
        {
            List<FilterDefinition<Item>> filters = new List<FilterDefinition<Item>>();
            if (!string.IsNullOrEmpty(request.TypeId))
                filters.Add(Builders<Item>.Filter.Eq("TypeId", new ObjectId(request.TypeId)));
            if (!string.IsNullOrEmpty(request.ParentId))
                filters.Add(Builders<Item>.Filter.Eq("ParentId", new ObjectId(request.ParentId)));
            if (!string.IsNullOrEmpty(request.Name))
                filters.Add(Builders<Item>.Filter.Regex("Name", new BsonRegularExpression($".*{request.Name}.*")));
            if (!string.IsNullOrEmpty(request.Json))
            {
                filters.Clear(); // For now, I see no reason to support both Json filtering and explicit field filtering
                filters.Add(new JsonFilterDefinition<Item>(request.Json));
            }

            try
            {
                var filterConcat = Builders<Item>.Filter.And(filters);

                ResetTemporaryCaches();

                var items = await _context.Items.Find(filterConcat).ToListAsync();
                List<Item> itemsToReturn = new List<Item>();
                
                foreach (var item in items)
                {   // Hydrate items before returning
                    itemsToReturn.Add(await HydrateForGetAndSave(item));
                }

                return itemsToReturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<Item> Add(Item item)
        {
            try
            {
                ResetTemporaryCaches();

                item = await HydrateForSave(item);

                await _context.Items.InsertOneAsync(item);
                return await Get(item.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Item> Update(Item item)
        {
            var filter = Builders<Item>.Filter.Eq("Id", item.Id);

            try
            {
                ResetTemporaryCaches();

                item = await HydrateForSave(item);

                // Compare old and new, save to change history
                var oldItem = await Get(item.Id);
                var newItem = item;
                List<FieldChange> fieldChanges = new List<FieldChange>();

                if (oldItem.Name != newItem.Name)
                {
                    fieldChanges.Add(new FieldChange()
                    {
                        FieldId = new ObjectId("000000000000000000000000"),
                        FieldName = "Name",
                        OldValue = oldItem.Name,
                        NewValue = newItem.Name
                    });
                }

                if (oldItem.TypeId != newItem.TypeId)
                {
                    fieldChanges.Add(new FieldChange()
                    {
                        FieldId = new ObjectId("000000000000000000000000"),
                        FieldName = "Type",
                        OldValue = oldItem.TypeId.ToString(),
                        NewValue = newItem.TypeId.ToString()
                    });
                }

                if (oldItem.WorkflowNode.Id != newItem.WorkflowNode.Id)
                {
                    fieldChanges.Add(new FieldChange()
                    {
                        FieldId = new ObjectId("000000000000000000000000"),
                        FieldName = "Status",
                        OldValue = (await _workflowNodeRepository.Get(oldItem.WorkflowNode.Id)).Name,
                        NewValue = (await _workflowNodeRepository.Get(newItem.WorkflowNode.Id)).Name
                    });
                }

                // Collect old values that have changed
                foreach (FieldValue oldFv in oldItem.FieldValues)
                {
                    var newFv = newItem.FieldValues.Where(f => f.FieldId == oldFv.FieldId).FirstOrDefault();

                    if (newFv == null || newFv.Value != oldFv.Value)
                    {
                        fieldChanges.Add(new FieldChange()
                        {
                            FieldId = oldFv.FieldId,
                            FieldName = oldFv.FieldName,
                            OldValue = oldFv.Value,
                            NewValue = newFv.Value
                        });
                    }
                }

                // Collect new values that weren't saved before
                foreach (FieldValue newFv in newItem.FieldValues)
                {
                    var oldFv = oldItem.FieldValues.Where(f => f.FieldId == newFv.FieldId).FirstOrDefault();

                    if (oldFv == null)
                    {
                        fieldChanges.Add(new FieldChange()
                        {
                            FieldId = newFv.FieldId,
                            FieldName = newFv.FieldName,
                            OldValue = "",
                            NewValue = newFv.Value
                        });
                    }
                }

                if (fieldChanges.Count > 0)
                {
                    await _fieldChangeGroupRepository.Add(new FieldChangeGroup()
                    {
                        ChangedByUserId = new ObjectId("000000000000000000000000"),
                        ChangedDateTime = DateTime.UtcNow,
                        ItemId = oldItem.Id,
                        FieldChanges = fieldChanges
                    });
                }

                await _context.Items.ReplaceOneAsync(filter, item);
                return await Get(item.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Delete(ObjectId id)
        {
            var filter = Builders<Item>.Filter.Eq("Id", id);

            try
            {
                await _context.Items.DeleteOneAsync(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
