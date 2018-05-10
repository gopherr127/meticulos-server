using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.Dashboard
{
    public class DashboardPanelRepository : IDashboardPanelRepository
    {
        private readonly DashboardPanelContext _context = null;

        public DashboardPanelRepository(IOptions<Settings> settings)
        {
            _context = new DashboardPanelContext(settings);
        }


        public async Task<IEnumerable<DashboardPanel>> GetAll()
        {
            try
            {
                return await _context.DashboardPanels.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                //TODO: log/manage exception
                throw ex;
            }
        }

        public async Task<DashboardPanel> Get(ObjectId id)
        {
            var filter = Builders<DashboardPanel>.Filter.Eq("_id", id);

            try
            {
                return await _context.DashboardPanels.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DashboardPanel> Add(DashboardPanel item)
        {
            try
            {
                await _context.DashboardPanels.InsertOneAsync(item);
                return await Get(item.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DashboardPanel> Update(DashboardPanel item)
        {
            var filter = Builders<DashboardPanel>.Filter.Eq("Id", item.Id);

            try
            {
                await _context.DashboardPanels.ReplaceOneAsync(filter, item);
                return await Get(item.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Delete(ObjectId id)
        {
            var filter = Builders<DashboardPanel>.Filter.Eq("Id", id);

            try
            {
                await _context.DashboardPanels.DeleteOneAsync(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
