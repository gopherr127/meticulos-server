using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Meticulos.Api.App.Dashboard
{
    public interface IDashboardPanelRepository
    {
        Task<DashboardPanel> Add(DashboardPanel item);
        Task Delete(ObjectId id);
        Task<DashboardPanel> Get(ObjectId id);
        Task<IEnumerable<DashboardPanel>> GetAll();
        Task<DashboardPanel> Update(DashboardPanel item);
    }
}