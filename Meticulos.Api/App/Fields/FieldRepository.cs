using Meticulos.Api.App.FieldOptions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.Fields
{
    public class FieldRepository : IFieldRepository
    {
        private readonly FieldContext _context = null;
        private readonly IFieldOptionRepository _fieldOptionRepository = null;

        public FieldRepository(IOptions<Settings> settings,
            IFieldOptionRepository fieldOptionRepository)
        {
            _context = new FieldContext(settings);
            _fieldOptionRepository = fieldOptionRepository;
        }

        public async Task<IEnumerable<Field>> GetAll()
        {
            try
            {
                return await _context.Fields.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Field> Get(ObjectId id)
        {
            var filter = Builders<Field>.Filter.Eq("_id", id);

            try
            {
                return await _context.Fields.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Field>> Find(List<ObjectId> fieldIds)
        {
            try
            {
                var filter = Builders<Field>.Filter.In("Id", fieldIds);
                return await _context.Fields.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Field> Add(Field field)
        {
            try
            {
                if (string.IsNullOrEmpty(field.Name))
                    throw new ApplicationException("Name field is required.");

                if (field.ValueOptions != null && 
                    (field.Type == FieldTypes.SingleSelectList
                    || field.Type == FieldTypes.MultiSelectList
                    || field.Type == FieldTypes.CheckboxList
                    || field.Type == FieldTypes.RadioList))
                {
                    List<FieldOption> options = new List<FieldOption>();
                    foreach(FieldOption opt in field.ValueOptions)
                    {
                        if (string.IsNullOrEmpty(opt.Name))
                            continue;

                        options.Add(await _fieldOptionRepository.Add(new FieldOption()
                        {
                            FieldId = field.Id,
                            Name = opt.Name
                        }));
                    }

                    field.ValueOptions = options;
                }

                await _context.Fields.InsertOneAsync(field);
                return await Get(field.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Field> Update(Field field)
        {
            var filter = Builders<Field>.Filter.Eq("Id", field.Id);

            try
            {
                if (string.IsNullOrEmpty(field.Name))
                    throw new ApplicationException("Name field is required.");

                var fieldToEdit = await Get(field.Id);
                if (fieldToEdit == null)
                    throw new ApplicationException("Unable to find field specified.");

                fieldToEdit.Name = field.Name;
                fieldToEdit.DefaultValue = field.DefaultValue;

                // Delete field options that were removed
                foreach (FieldOption fo in fieldToEdit.ValueOptions)
                {
                    if (!field.ValueOptions.Contains(fo))
                    {
                        await _fieldOptionRepository.Delete(fo.Id);
                    }
                }

                if (field.ValueOptions != null &&
                    (field.Type == FieldTypes.SingleSelectList
                    || field.Type == FieldTypes.MultiSelectList
                    || field.Type == FieldTypes.CheckboxList
                    || field.Type == FieldTypes.RadioList))
                {
                    List<FieldOption> options = new List<FieldOption>();
                    foreach (FieldOption opt in field.ValueOptions)
                    {
                        if (string.IsNullOrEmpty(opt.Name))
                            continue;

                        if (opt.Id == ObjectId.Empty)
                        {
                            options.Add(await _fieldOptionRepository.Add(new FieldOption()
                            {
                                FieldId = field.Id,
                                Name = opt.Name
                            }));
                        }
                        else
                        {
                            options.Add(await _fieldOptionRepository.Update(opt));
                        }
                    }

                    fieldToEdit.ValueOptions = options;
                }

                await _context.Fields.ReplaceOneAsync(filter, fieldToEdit);
                return await Get(field.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Delete(ObjectId id)
        {
            var filter = Builders<Field>.Filter.Eq("Id", id);

            try
            {
                var fieldToDelete = await Get(id);

                if (fieldToDelete.ValueOptions != null)
                {
                    foreach (FieldOption fo in fieldToDelete.ValueOptions)
                    {
                        await _fieldOptionRepository.Delete(fo.Id);
                    }
                }

                await _context.Fields.DeleteOneAsync(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
