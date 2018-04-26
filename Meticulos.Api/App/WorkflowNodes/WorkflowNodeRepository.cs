using Meticulos.Api.App.Workflows;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.WorkflowNodes
{
    public class WorkflowNodeRepository : IWorkflowNodeRepository
    {
        private readonly WorkflowNodeContext _context = null;

        public WorkflowNodeRepository(IOptions<Settings> settings)
        {
            _context = new WorkflowNodeContext(settings);
        }

        public async Task<IEnumerable<WorkflowNode>> GetAll()
        {
            try
            {
                return await _context.WorkflowNodes.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                //TODO: log/manage exception
                throw ex;
            }
        }

        public async Task<IEnumerable<WorkflowNode>> Search(ObjectId workflowId)
        {
            var filter = Builders<WorkflowNode>.Filter.Eq("WorkflowId", workflowId);

            try
            {
                return await _context.WorkflowNodes.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<WorkflowNode> Get(ObjectId id)
        {
            var filter = Builders<WorkflowNode>.Filter.Eq("Id", id);

            try
            {
                return await _context.WorkflowNodes.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<WorkflowNode> Add(WorkflowNode item)
        {
            try
            {
                if (item.WorkflowId == null || item.WorkflowId == ObjectId.Empty)
                    throw new ApplicationException("WorkflowId is required.");

                await _context.WorkflowNodes.InsertOneAsync(item);
                return await Get(item.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<WorkflowNode> Update(WorkflowNode item)
        {
            var filter = Builders<WorkflowNode>.Filter.Eq("Id", item.Id);

            try
            {
                await _context.WorkflowNodes.ReplaceOneAsync(filter, item);
                return await Get(item.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Delete(ObjectId id)
        {
            var filter = Builders<WorkflowNode>.Filter.Eq("Id", id);

            try
            {
                await _context.WorkflowNodes.DeleteOneAsync(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
