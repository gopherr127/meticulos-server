using Meticulos.Api.App.Fields;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.Screens
{
    public class ScreenRepository : IScreenRepository
    {
        private readonly ScreenContext _context = null;
        private readonly IFieldRepository _fieldRepository;

        public ScreenRepository(IOptions<Settings> settings,
            IFieldRepository fieldRepository)
        {
            _context = new ScreenContext(settings);
            _fieldRepository = fieldRepository;
        }

        public async Task<IEnumerable<Screen>> GetAll()
        {
            try
            {
                return await _context.Screens.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<Screen> Hydrate(Screen screen)
        {
            foreach (FieldMetadata fm in screen.Fields)
            {
                var field = await _fieldRepository.Get(fm.Id);
                if (field != null)
                {
                    fm.Name = field.Name;
                    fm.Type = field.Type;
                    fm.ValueOptions = field.ValueOptions;
                    fm.DefaultValue = field.DefaultValue;
                }
            }

            return screen;
        }

        public async Task<Screen> Get(ObjectId id)
        {
            var filter = Builders<Screen>.Filter.Eq("_id", id);

            try
            {
                var screen = await _context.Screens.Find(filter).FirstOrDefaultAsync();
                return await Hydrate(screen);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Screen>> Find(List<ObjectId> screenIds)
        {
            try
            {
                var filter = Builders<Screen>.Filter.In("Id", screenIds);
                return await _context.Screens.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Screen> Add(Screen screen)
        {
            try
            {
                var hydratedScreen = await Hydrate(screen);
                await _context.Screens.InsertOneAsync(hydratedScreen);
                return await Get(screen.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Screen> Update(Screen screen)
        {
            var filter = Builders<Screen>.Filter.Eq("Id", screen.Id);

            try
            {
                var hydratedScreen = await Hydrate(screen);
                await _context.Screens.ReplaceOneAsync(filter, hydratedScreen);
                return await Get(screen.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Delete(ObjectId id)
        {
            var filter = Builders<Screen>.Filter.Eq("Id", id);

            try
            {
                await _context.Screens.DeleteOneAsync(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
