using Meticulos.Api.App.WorkflowFunctions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.WorkflowTransitionFunctions
{
    public class WorkflowTransitionFunctionRepository : IWorkflowTransitionFunctionRepository
    {
        private readonly WorkflowTransitionFunctionContext _context = null;
        private readonly IWorkflowFunctionRepository _workflowFunctionRepository;

        public WorkflowTransitionFunctionRepository(IOptions<Settings> settings,
            IWorkflowFunctionRepository workflowFunctionRepository)
        {
            _context = new WorkflowTransitionFunctionContext(settings);
            _workflowFunctionRepository = workflowFunctionRepository;
        }

        public async Task<IEnumerable<WorkflowTransitionFunction>> GetAll()
        {
            try
            {
                return await _context.WorkflowTransitionFunctions.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<WorkflowTransitionFunction>> Search(WorkflowTransitionFunctionSearchRequest requestArgs)
        {
            if (string.IsNullOrEmpty(requestArgs.TransitionId))
                throw new ApplicationException("Required fields not supplied.");

            var filter = Builders<WorkflowTransitionFunction>.Filter.Eq("TransitionId", new ObjectId(requestArgs.TransitionId));

            try
            {
                var transitionFunctions = await _context.WorkflowTransitionFunctions.Find(filter).ToListAsync();

                // Hydrate Function for all transition functions
                Dictionary<string, WorkflowFunction> functionCache = new Dictionary<string, WorkflowFunction>();
                foreach (var transFunction in transitionFunctions)
                {
                    string funcId = transFunction.FunctionId.ToString();

                    if (!functionCache.ContainsKey(funcId))
                    {
                        transFunction.Function = await _workflowFunctionRepository.Get(transFunction.FunctionId);
                    }
                    else
                    {
                        transFunction.Function = functionCache[funcId];
                    }
                }

                return transitionFunctions;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<WorkflowTransitionFunction> Get(ObjectId id)
        {
            var filter = Builders<WorkflowTransitionFunction>.Filter.Eq("Id", id);

            try
            {
                var transition = await _context.WorkflowTransitionFunctions.Find(filter).FirstOrDefaultAsync();

                return await Hydrate(transition);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<WorkflowTransitionFunction> Hydrate(WorkflowTransitionFunction item)
        {
            try
            {
                if (item.FunctionId == ObjectId.Empty || item.TransitionId == ObjectId.Empty)
                    throw new ApplicationException("Required fields missing.");
                if (string.IsNullOrEmpty(item.FunctionArgs))
                    throw new ApplicationException("Required fields missing.");
                var func = await _workflowFunctionRepository.Get(item.FunctionId);
                item.Function = func ?? throw new ApplicationException("Cannot find function specified.");

                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<WorkflowTransitionFunction> Add(WorkflowTransitionFunction item)
        {
            try
            {
                item = await Hydrate(item);

                await _context.WorkflowTransitionFunctions.InsertOneAsync(item);
                return await Get(item.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<WorkflowTransitionFunction> Update(WorkflowTransitionFunction item)
        {
            var filter = Builders<WorkflowTransitionFunction>.Filter.Eq("Id", item.Id);

            try
            {
                item = await Hydrate(item);

                await _context.WorkflowTransitionFunctions.ReplaceOneAsync(filter, item);
                return await Get(item.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Delete(ObjectId id)
        {
            var filter = Builders<WorkflowTransitionFunction>.Filter.Eq("Id", id);

            try
            {
                await _context.WorkflowTransitionFunctions.DeleteOneAsync(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
