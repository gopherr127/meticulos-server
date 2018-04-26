using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Meticulos.Api.App.Items
{
    public class ItemContext : ContextBase
    {
        public ItemContext(IOptions<Settings> settings) : base(settings) { }
        
        public IMongoCollection<Item> Items
        {
            get { return Database.GetCollection<Item>("Items"); }
        }
    }
}
