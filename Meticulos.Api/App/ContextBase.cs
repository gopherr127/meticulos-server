using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Security.Authentication;

namespace Meticulos.Api.App
{
    public abstract class ContextBase
    {
        private readonly IOptions<Settings> _settings;
        public ContextBase(IOptions<Settings> settings)
        {
            _settings = settings;
        }

        private IMongoDatabase _database = null;
        public IMongoDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    string connectionString = _settings.Value.ConnectionString;
                    MongoClientSettings settings = MongoClientSettings.FromUrl(
                      new MongoUrl(connectionString)
                    );
                    settings.SslSettings =
                      new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
                    var client = new MongoClient(settings);

                    //var client = new MongoClient(_settings.Value.ConnectionString);
                    if (client == null)
                    {
                        //TODO: handle error/throw exception
                    }
                    _database = client.GetDatabase(_settings.Value.Database);
                }

                return _database;
            }
        }
    }
}
