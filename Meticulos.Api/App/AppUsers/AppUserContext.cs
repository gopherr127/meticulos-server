using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Meticulos.Api.App.AppUsers
{
    public class AppUserContext : ContextBase
    {
        private IMongoDatabase _database = null;

        public AppUserContext(IOptions<Settings> settings) : base(settings) {}

        public IMongoCollection<AppUser> AppUsers
        {
            get
            {
                if (_database == null)
                {
                    _database = Client.GetDatabase("UserRegistry");
                }

                return _database.GetCollection<AppUser>("AppUsers");
            }
        }
    }
}
