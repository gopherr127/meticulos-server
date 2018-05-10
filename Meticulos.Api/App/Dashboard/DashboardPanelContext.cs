using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Meticulos.Api.App.Dashboard
{
    public class DashboardPanelContext : ContextBase
    {
        public DashboardPanelContext(IOptions<Settings> settings) : base(settings) { }

        public IMongoCollection<DashboardPanel> DashboardPanels
        {
            get { return Database.GetCollection<DashboardPanel>("DashboardPanels"); }
        }
    }
}
