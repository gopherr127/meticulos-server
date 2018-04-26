using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.FieldOptions
{
    public class FieldOptionRepository : IFieldOptionRepository
    {
        private readonly FieldOptionContext _context = null;

        public FieldOptionRepository(IOptions<Settings> settings)
        {
            _context = new FieldOptionContext(settings);
        }

        public async Task<FieldOption> Get(ObjectId id)
        {
            var filter = Builders<FieldOption>.Filter.Eq("_id", id);

            try
            {
                return await _context.FieldOptions.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<FieldOption>> Search(ObjectId fieldId)
        {
            var filter = Builders<FieldOption>.Filter.Eq("FieldId", fieldId);

            try
            {
                return await _context.FieldOptions.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<FieldOption> Add(FieldOption fieldOption)
        {
            try
            {
                if (string.IsNullOrEmpty(fieldOption.Name))
                    throw new ApplicationException("Name field is required.");

                await _context.FieldOptions.InsertOneAsync(fieldOption);
                return await Get(fieldOption.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<FieldOption> Update(FieldOption fieldOption)
        {
            var filter = Builders<FieldOption>.Filter.Eq("Id", fieldOption.Id);

            try
            {
                // Only accepting changes to Name
                var fieldToEdit = await Get(fieldOption.Id);
                if (fieldToEdit == null)
                    throw new ApplicationException("Unable to find field specified.");
                
                fieldToEdit.Name = fieldOption.Name;
                
                await _context.FieldOptions.ReplaceOneAsync(filter, fieldToEdit);
                return await Get(fieldOption.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Delete(ObjectId id)
        {
            var filter = Builders<FieldOption>.Filter.Eq("Id", id);

            try
            {
                await _context.FieldOptions.DeleteOneAsync(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
