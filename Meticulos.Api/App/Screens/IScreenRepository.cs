using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Meticulos.Api.App.Screens
{
    public interface IScreenRepository
    {
        Task<IEnumerable<Screen>> GetAll();
        Task<Screen> Get(ObjectId id);
        Task<List<Screen>> Find(List<ObjectId> screenIds);
        Task<Screen> Add(Screen item);
        Task<Screen> Update(Screen item);
        Task Delete(ObjectId id);
    }
}