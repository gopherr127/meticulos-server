using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Meticulos.Api.App.FieldOptions
{
    public class FieldOptionContext : ContextBase
    {
        public FieldOptionContext(IOptions<Settings> settings) : base(settings) { }

        public IMongoCollection<FieldOption> FieldOptions
        {
            get { return Database.GetCollection<FieldOption>("FieldOptions"); }
        }
    }
}
