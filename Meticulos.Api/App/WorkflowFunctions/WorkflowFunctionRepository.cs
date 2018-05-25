using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.WorkflowFunctions
{
    public class WorkflowFunctionRepository : IWorkflowFunctionRepository
    {
        private readonly WorkflowFunctionContext _context = null;

        public WorkflowFunctionRepository(IOptions<Settings> settings)
        {
            _context = new WorkflowFunctionContext(settings);
        }
        
        public async Task<WorkflowFunction> Get(ObjectId id)
        {
            var filter = Builders<WorkflowFunction>.Filter.Eq("Id", id);

            try
            {
                return await _context.WorkflowFunctions.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<WorkflowFunction>> Search(WorkflowFunctionSearchRequest requestArgs)
        {
            if (!Enum.IsDefined(typeof(WorkflowFunctionTypes), requestArgs.Type))
                throw new ApplicationException("Invalid function type specified.");

            var filter = Builders<WorkflowFunction>.Filter.Eq("Type", requestArgs.Type);

            try
            {
                return await _context.WorkflowFunctions.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Add(WorkflowFunction item)
        {
            try
            {
                await _context.WorkflowFunctions.InsertOneAsync(item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
