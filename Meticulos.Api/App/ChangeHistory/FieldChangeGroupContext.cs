using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Meticulos.Api.App.ChangeHistory
{
    public class FieldChangeGroupContext : ContextBase
    {
        public FieldChangeGroupContext(IOptions<Settings> settings) : base(settings) { }
        
        public IMongoCollection<FieldChangeGroup> FieldChangeGroups
        {
            get { return Database.GetCollection<FieldChangeGroup>("FieldChangeGroups"); }
        }
    }
}
