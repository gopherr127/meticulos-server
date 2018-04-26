using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.ItemTypes
{
    public class ItemTypeRepository : IItemTypeRepository
    {
        private readonly ItemTypeContext _context = null;

        public ItemTypeRepository(IOptions<Settings> settings)
        {
            _context = new ItemTypeContext(settings);
        }

        public async Task<IEnumerable<ItemType>> GetAll()
        {
            try
            {
                return await _context.ItemTypes.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                //TODO: log/manage exception
                throw ex;
            }
        }

        public async Task<ItemType> Get(ObjectId id)
        {
            var filter = Builders<ItemType>.Filter.Eq("Id", id);

            try
            {
                return await _context.ItemTypes.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                //TODO: log/manage exception
                throw ex;
            }
        }

        public async Task<List<ItemType>> Search(ItemTypeSearchRequest request)
        {
            List<FilterDefinition<ItemType>> filters = new List<FilterDefinition<ItemType>>();
            if (!string.IsNullOrEmpty(request.Name))
                filters.Add(Builders<ItemType>.Filter.Eq("Name", request.Name));
            if (request.IsForPysicalItems != null)
                filters.Add(Builders<ItemType>.Filter.Eq("IsForPysicalItems", request.IsForPysicalItems));
            if (request.AllowNestedItems != null)
                filters.Add(Builders<ItemType>.Filter.Eq("AllowNestedItems", request.AllowNestedItems));

            try
            {
                var filterConcat = Builders<ItemType>.Filter.And(filters);
                return await _context.ItemTypes.Find(filterConcat).ToListAsync();
            }
            catch (Exception ex)
            {
                //TODO: log/manage exception
                throw ex;
            }
        }

        public async Task<ItemType> Add(ItemType item)
        {
            try
            {
                await _context.ItemTypes.InsertOneAsync(item);
                return await Get(item.Id);
            }
            catch (Exception ex)
            {
                //TODO: log/manage exception
                throw ex;
            }
        }

        public async Task<ItemType> Update(ItemType item)
        {
            var filter = Builders<ItemType>.Filter.Eq("Id", item.Id);

            try
            {
                await _context.ItemTypes.ReplaceOneAsync(filter, item);
                return await Get(item.Id);
            }
            catch (Exception ex)
            {
                //TODO: log/manage exception
                throw ex;
            }
        }

        public async Task Delete(ObjectId id)
        {
            var filter = Builders<ItemType>.Filter.Eq("Id", id);

            try
            {
                await _context.ItemTypes.DeleteOneAsync(filter);
            }
            catch (Exception ex)
            {
                //TODO: log/manage exception
                throw ex;
            }
        }
    }
}
