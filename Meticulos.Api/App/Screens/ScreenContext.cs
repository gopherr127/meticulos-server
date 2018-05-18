using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Meticulos.Api.App.Screens
{
    public class ScreenContext : ContextBase
    {
        public ScreenContext(IOptions<Settings> settings) : base(settings) { }

        public IMongoCollection<Screen> Screens
        {
            get { return DefaultDatabase.GetCollection<Screen>("Screens"); }
        }
    }
}
