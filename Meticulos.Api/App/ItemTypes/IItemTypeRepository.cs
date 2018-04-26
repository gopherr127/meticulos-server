using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.ItemTypes
{
    public interface IItemTypeRepository
    {
        Task<IEnumerable<ItemType>> GetAll();
        Task<ItemType> Get(ObjectId id);
        Task<List<ItemType>> Search(ItemTypeSearchRequest request);
        Task<ItemType> Add(ItemType item);
        Task<ItemType> Update(ItemType item);
        Task Delete(ObjectId id);
    }
}