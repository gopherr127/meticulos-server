using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.Workflows
{
    public class WorkflowRepository : IWorkflowRepository
    {
        private readonly WorkflowContext _context = null;

        public WorkflowRepository(IOptions<Settings> settings)
        {
            _context = new WorkflowContext(settings);
        }

        public async Task<IEnumerable<Workflow>> GetAll()
        {
            try
            {
                return await _context.Workflows.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                //TODO: log/manage exception
                throw ex;
            }
        }

        public async Task<Workflow> Get(ObjectId id)
        {
            var filter = Builders<Workflow>.Filter.Eq("_id", id);

            try
            {
                return await _context.Workflows.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Workflow> Add(Workflow item)
        {
            try
            {
                await _context.Workflows.InsertOneAsync(item);
                return await Get(item.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Workflow> Update(Workflow item)
        {
            var filter = Builders<Workflow>.Filter.Eq("Id", item.Id);

            try
            {
                await _context.Workflows.ReplaceOneAsync(filter, item);
                return await Get(item.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Delete(ObjectId id)
        {
            var filter = Builders<Workflow>.Filter.Eq("Id", id);

            try
            {
                await _context.Workflows.DeleteOneAsync(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
