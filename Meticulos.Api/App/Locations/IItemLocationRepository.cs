using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Meticulos.Api.App.Locations
{
    public interface IItemLocationRepository
    {
        Task<ItemLocation> Add(ItemLocation location);
        Task Delete(ObjectId id);
        Task<ItemLocation> Get(ObjectId id);
        Task<IEnumerable<ItemLocation>> GetAll();
        Task<IEnumerable<ItemLocation>> Search(ObjectId itemLocationId);
        Task<ItemLocation> Update(ItemLocation location);
    }
}