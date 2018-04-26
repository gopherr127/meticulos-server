using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Meticulos.Api.App.ChangeHistory
{
    public interface IFieldChangeGroupRepository
    {
        Task<FieldChangeGroup> Get(ObjectId id);
        Task<IEnumerable<FieldChangeGroup>> Getall();
        Task<IEnumerable<FieldChangeGroup>> Search(ObjectId itemId);
        Task<FieldChangeGroup> Add(FieldChangeGroup item);
    }
}