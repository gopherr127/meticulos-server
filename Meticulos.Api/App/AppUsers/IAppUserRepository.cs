using System.Threading.Tasks;
using MongoDB.Bson;

namespace Meticulos.Api.App.AppUsers
{
    public interface IAppUserRepository
    {
        //Task<AppUser> Authenticate(AppUser user);
        //Task<AppUser> Add(AppUser user);
        Task Delete(ObjectId id);
        Task<AppUser> Get(ObjectId id);
        Task<AppUser> Update(AppUser user);
    }
}