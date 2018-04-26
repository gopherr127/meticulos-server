using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Meticulos.Api.App.Fields
{
    public class FieldContext : ContextBase
    {
        public FieldContext(IOptions<Settings> settings) : base(settings) { }

        public IMongoCollection<Field> Fields
        {
            get { return Database.GetCollection<Field>("Fields"); }
        }
    }
}