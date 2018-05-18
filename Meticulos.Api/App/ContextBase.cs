using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Security.Authentication;

namespace Meticulos.Api.App
{
    public abstract class ContextBase
    {
        private readonly IOptions<Settings> _settings;
        private MongoClient _client = null;
        private IMongoDatabase _database = null;

        public ContextBase(IOptions<Settings> settings)
        {
            _settings = settings;
        }

        public MongoClient Client
        {
            get
            {
                if (_client == null)
                {
                    MongoClientSettings settings = MongoClientSettings.FromUrl(
                      new MongoUrl(_settings.Value.ConnectionString)
                    );

                    settings.SslSettings =
                      new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

                    _client = new MongoClient(settings);

                    if (_client == null)
                    {
                        throw new ApplicationException("Unable to instantiate database client.");
                    }
                }

                return _client;
            }
        }

        public IMongoDatabase DefaultDatabase
        {
            get
            {
                if (_database == null)
                {
                    _database = Client.GetDatabase(_settings.Value.DefaultDatabase);
                }

                return _database;
            }
        }
    }
}
