using Meticulos.Api.App.ChangeHistory;
using Meticulos.Api.App.Fields;
using Meticulos.Api.App.ItemTypes;
using Meticulos.Api.App.WorkflowNodes;
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
        private readonly IFieldChangeGroupRepository _fieldChangeGroupRepository;

        public ItemRepository(IOptions<Settings> settings,
            IItemTypeRepository itemTypeRepository,
            IWorkflowNodeRepository workflowNodeRepository,
            IFieldChangeGroupRepository fieldChangeGroupRepository)
        {
            _context = new ItemContext(settings);
            _itemTypeRepository = itemTypeRepository;
            _workflowNodeRepository = workflowNodeRepository;
            _fieldChangeGroupRepository = fieldChangeGroupRepository;
        }


        private async Task<Item> HydrateForGetAndSave(Item item)
        {
            try
            {
                // Set Ancestor IDs
                List<ObjectId> ancestorIds = new List<ObjectId>();
                if (item.ParentId != ObjectId.Empty)
                {
                    var parent = await Get(item.ParentId);
                    if (parent == null)
                        throw new ApplicationException("Unable to find parent specified.");
                    ancestorIds.AddRange(parent.AncestorIds);
                }
                ancestorIds.Add(item.ParentId);
                item.AncestorIds = ancestorIds;

                // Set Type from ID
                var itemType = await _itemTypeRepository.Get(item.TypeId);
                item.Type = itemType ?? throw new ApplicationException("Invalid Type ID.");

                if (item.WorkflowNode == null)
                {
                    // Set Workflow Node from ItemType's associated Workflow
                    var workflowNodes = await _workflowNodeRepository.Search(itemType.WorkflowId);
                    item.WorkflowNode = workflowNodes.FirstOrDefault();
                }

                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<Item> HydrateForGet(Item item)
        {
            try
            {
                item = await HydrateForGetAndSave(item);

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

        public async Task<Item> Get(ObjectId id)
        {
            var filter = Builders<Item>.Filter.Eq("Id", id);

            try
            {
                var result = await _context.Items.Find(filter).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Item>> Search(string name)
        {
            var filter = Builders<Item>.Filter.Eq("Name", name);

            try
            {
                return await _context.Items.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Item>> Search(ObjectId parentId)
        {
            var filter = Builders<Item>.Filter.Eq("ParentId", parentId);

            try
            {
                var items = await _context.Items.Find(filter).ToListAsync();

                // Hydrate ItemType for all Items
                Dictionary<string, ItemType> itemTypeCache = new Dictionary<string, ItemType>();
                foreach (var item in items)
                {
                    string typeId = item.TypeId.ToString();

                    if (!itemTypeCache.ContainsKey(typeId))
                    {
                        item.Type = await _itemTypeRepository.Get(item.TypeId);
                    }
                    else
                    {
                        item.Type = itemTypeCache[typeId];
                    }
                }
                return items;
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
                item = await HydrateForSave(item);

                //TODO: Move this to a parallel task
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
                        FieldName = "WorkflowNode",
                        OldValue = oldItem.WorkflowNode.Id.ToString(),
                        NewValue = newItem.WorkflowNode.Id.ToString()
                    });
                }

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
