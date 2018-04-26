using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Meticulos.Api.App.FieldOptions
{
    public interface IFieldOptionRepository
    {
        Task<FieldOption> Get(ObjectId id);
        Task<List<FieldOption>> Search(ObjectId fieldId);
        Task<FieldOption> Add(FieldOption fieldOption);
        Task<FieldOption> Update(FieldOption fieldOption);
        Task Delete(ObjectId id);
    }
}