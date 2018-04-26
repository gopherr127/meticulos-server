using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Meticulos.Api.App.Fields
{
    public interface IFieldRepository
    {
        Task<IEnumerable<Field>> GetAll();
        Task<Field> Get(ObjectId id);
        Task<List<Field>> Find(List<ObjectId> fieldIds);
        Task<Field> Add(Field field);
        Task<Field> Update(Field field);
        Task Delete(ObjectId id);
    }
}