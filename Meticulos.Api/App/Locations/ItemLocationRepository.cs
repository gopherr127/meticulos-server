using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.Locations
{
    public class ItemLocationRepository : IItemLocationRepository
    {
        private readonly ItemLocationContext _context = null;

        public ItemLocationRepository(IOptions<Settings> settings)
        {
            _context = new ItemLocationContext(settings);
        }

        private async Task<ItemLocation> HydrateForGetAndSave(ItemLocation location)
        {
            try
            {
                if (location.ParentId != null && location.ParentId != ObjectId.Empty)
                {
                    var parent = await Get(location.ParentId);
                    location.Parent = parent
                        ?? throw new ApplicationException("Could not find parent location.");
                }

                return location;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ItemLocation>> GetAll()
        {
            try
            {
                var locations = await _context.ItemLocations.Find(_ => true).ToListAsync();
                //TODO: Hydrate locations
                return locations;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ItemLocation>> Search(ObjectId itemLocationId)
        {
            var filter = Builders<ItemLocation>.Filter.Eq("ItemLocationId", itemLocationId);

            try
            {
                var locations = await _context.ItemLocations.Find(filter).ToListAsync();
                //TODO: Hydrate locations
                return locations;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ItemLocation> Get(ObjectId id)
        {
            var filter = Builders<ItemLocation>.Filter.Eq("Id", id);

            try
            {
                var location = await _context.ItemLocations.Find(filter).FirstOrDefaultAsync();
                return await HydrateForGetAndSave(location);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ItemLocation> Add(ItemLocation location)
        {
            try
            {
                if (string.IsNullOrEmpty(location.Name))
                    throw new ApplicationException("Name is required.");

                location = await HydrateForGetAndSave(location);
                await _context.ItemLocations.InsertOneAsync(location);
                return await Get(location.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ItemLocation> Update(ItemLocation location)
        {
            var filter = Builders<ItemLocation>.Filter.Eq("Id", location.Id);

            try
            {
                location = await HydrateForGetAndSave(location);
                await _context.ItemLocations.ReplaceOneAsync(filter, location);
                return await Get(location.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Delete(ObjectId id)
        {
            var filter = Builders<ItemLocation>.Filter.Eq("Id", id);

            try
            {
                await _context.ItemLocations.DeleteOneAsync(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
