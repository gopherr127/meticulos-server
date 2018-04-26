using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Meticulos.Api.App.ItemTypes
{
    public class ItemTypeContext : ContextBase
    {
        public ItemTypeContext(IOptions<Settings> settings) : base(settings) { }

        public IMongoCollection<ItemType> ItemTypes
        {
            get { return Database.GetCollection<ItemType>("ItemTypes"); }
        }
    }
}
