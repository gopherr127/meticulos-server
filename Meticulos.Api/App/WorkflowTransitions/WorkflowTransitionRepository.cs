using Meticulos.Api.App.Screens;
using Meticulos.Api.App.WorkflowNodes;
using Meticulos.Api.App.WorkflowTransitionFunctions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meticulos.Api.App.WorkflowTransitions
{
    public class WorkflowTransitionRepository : IWorkflowTransitionRepository
    {
        private readonly WorkflowTransitionContext _context = null;
        private readonly IWorkflowNodeRepository _workflowNodeRepository;
        private readonly IWorkflowTransitionFunctionRepository _workflowTransitionFunctionRepository;
        private readonly IScreenRepository _screenRepository;

        public WorkflowTransitionRepository(IOptions<Settings> settings,
            IWorkflowNodeRepository workflowNodeRepository,
            IWorkflowTransitionFunctionRepository workflowTransitionFunctionRepository,
            IScreenRepository screenRepository)
        {
            _context = new WorkflowTransitionContext(settings);
            _workflowNodeRepository = workflowNodeRepository;
            _workflowTransitionFunctionRepository = workflowTransitionFunctionRepository;
            _screenRepository = screenRepository;
        }
        
        public async Task<IEnumerable<WorkflowTransition>> GetAll()
        {
            try
            {
                return await _context.WorkflowTransitions.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<WorkflowTransition>> Search(WorkflowTransitionSearchRequest requestArgs)
        {
            List<FilterDefinition<WorkflowTransition>> filters = new List<FilterDefinition<WorkflowTransition>>();
            if (!string.IsNullOrEmpty(requestArgs.WorkflowId))
                filters.Add(Builders<WorkflowTransition>.Filter.Eq("WorkflowId", new ObjectId(requestArgs.WorkflowId)));
            if (!string.IsNullOrEmpty(requestArgs.FromNodeId))
                filters.Add(Builders<WorkflowTransition>.Filter.Eq("FromNodeId", new ObjectId(requestArgs.FromNodeId)));

            try
            {
                var filterConcat = Builders<WorkflowTransition>.Filter.And(filters);
                return await _context.WorkflowTransitions.Find(filterConcat).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<WorkflowTransition> Get(ObjectId id)
        {
            var filter = Builders<WorkflowTransition>.Filter.Eq("Id", id);

            try
            {
                // See TODO up above for refactoring effort
                var transition = await _context.WorkflowTransitions.Find(filter).FirstOrDefaultAsync();
                transition = await HydrateForGet(transition);
                return transition;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<WorkflowTransition> HydrateForGetAndSave(WorkflowTransition transition)
        {
            try
            {
                var fromNode = await _workflowNodeRepository.Get(transition.FromNodeId);
                var toNode = await _workflowNodeRepository.Get(transition.ToNodeId);

                if (fromNode == null || toNode == null)
                {
                    throw new ApplicationException("Cannot find node.");
                }

                transition.FromNode = fromNode;
                transition.ToNode = toNode;

                return transition;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<WorkflowTransition> HydrateForGet(WorkflowTransition transition)
        {
            try
            {
                transition = await HydrateForGetAndSave(transition);

                var transFunctions = await _workflowTransitionFunctionRepository.Search(
                    new WorkflowTransitionFunctionSearchRequest() { TransitionId = transition.Id.ToString() });

                transition.PreConditions = transFunctions
                    .Where(f => f.Function.Type == WorkflowFunctions.WorkflowFunctionTypes.PreCondition).ToList();

                transition.Validations = transFunctions
                    .Where(f => f.Function.Type == WorkflowFunctions.WorkflowFunctionTypes.Validation).ToList();

                transition.PostFunctions = transFunctions
                    .Where(f => f.Function.Type == WorkflowFunctions.WorkflowFunctionTypes.PostFunction).ToList();

                if (transition.ScreenIds != null)
                {
                    transition.Screens = await _screenRepository.Find(transition.ScreenIds);
                }

                return transition;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<WorkflowTransition> HydrateForSave(WorkflowTransition transition)
        {
            try
            {
                transition = await HydrateForGetAndSave(transition);

                return transition;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<WorkflowTransition> Add(WorkflowTransition transition)
        {
            try
            {
                if (string.IsNullOrEmpty(transition.Name)
                    || transition.WorkflowId == ObjectId.Empty
                    || transition.FromNodeId == ObjectId.Empty
                    || transition.ToNodeId == ObjectId.Empty)
                {
                    throw new ApplicationException("Required fields missing.");
                }

                transition = await HydrateForSave(transition);

                await _context.WorkflowTransitions.InsertOneAsync(transition);
                return await Get(transition.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<WorkflowTransition> Update(WorkflowTransition transition)
        {
            var filter = Builders<WorkflowTransition>.Filter.Eq("Id", transition.Id);

            try
            {
                if (string.IsNullOrEmpty(transition.Name)
                    || transition.WorkflowId == ObjectId.Empty
                    || transition.FromNodeId == ObjectId.Empty
                    || transition.ToNodeId == ObjectId.Empty)
                {
                    throw new ApplicationException("Required fields missing.");
                }

                transition = await HydrateForSave(transition);

                await _context.WorkflowTransitions.ReplaceOneAsync(filter, transition);
                return await Get(transition.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Delete(ObjectId id)
        {
            var filter = Builders<WorkflowTransition>.Filter.Eq("Id", id);

            try
            {
                await _context.WorkflowTransitions.DeleteOneAsync(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
