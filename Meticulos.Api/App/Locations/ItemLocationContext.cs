using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Meticulos.Api.App.Locations
{
    public class ItemLocationContext : ContextBase
    {
        public ItemLocationContext(IOptions<Settings> settings) : base(settings) { }

        public IMongoCollection<ItemLocation> ItemLocations
        {
            get { return Database.GetCollection<ItemLocation>("ItemLocations"); }
        }
    }
}
