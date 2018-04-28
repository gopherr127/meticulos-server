using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Meticulos.Api.App.Items
{
    public interface IItemRepository
    {
        Task<Item> Add(Item item);
        Task Delete(ObjectId id);
        Task<Item> Get(ObjectId id);
        Task<IEnumerable<Item>> GetAll();
        Task<List<Item>> Search(ItemSearchRequest request);
        Task<Item> Update(Item item);
    }
}