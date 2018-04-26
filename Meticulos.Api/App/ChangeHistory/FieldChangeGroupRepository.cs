using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.ChangeHistory
{
    public class FieldChangeGroupRepository : IFieldChangeGroupRepository
    {
        private readonly FieldChangeGroupContext _context = null;

        public FieldChangeGroupRepository(IOptions<Settings> settings)
        {
            _context = new FieldChangeGroupContext(settings);
        }

        public async Task<IEnumerable<FieldChangeGroup>> Getall()
        {
            try
            {
                return await _context.FieldChangeGroups.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<FieldChangeGroup> Get(ObjectId id)
        {
            var filter = Builders<FieldChangeGroup>.Filter.Eq("Id", id);

            try
            {
                var result = await _context.FieldChangeGroups.Find(filter).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<FieldChangeGroup>> Search(ObjectId itemId)
        {
            var filter = Builders<FieldChangeGroup>.Filter.Eq("ItemId", itemId);

            try
            {
                return await _context.FieldChangeGroups.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<FieldChangeGroup> Add(FieldChangeGroup item)
        {
            try
            {
                await _context.FieldChangeGroups.InsertOneAsync(item);
                return await Get(item.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
