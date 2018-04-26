using Meticulos.Api.App.WorkflowFunctions.PreConditions;
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
        
        private async Task CreateDefaultFunctions()
        {
            List<WorkflowFunction> defaultFunctions = new List<WorkflowFunction>();
            
            defaultFunctions.Add(new WorkflowFunction()
            {
                Id = ObjectId.Parse("5aa805dd0af6814a103b25ad"),
                Type = WorkflowFunctionTypes.PreCondition,
                Name = "User is in Role"
            });
            defaultFunctions.Add(new WorkflowFunction()
            {
                Id = ObjectId.Parse("5aa805dd0af6814a103b25ae"),
                Type = WorkflowFunctionTypes.PreCondition,
                Name = "User is in Group"
            });
            defaultFunctions.Add(new WorkflowFunction()
            {
                Id = ObjectId.Parse("5aa805de0af6814a103b25af"),
                Type = WorkflowFunctionTypes.PreCondition,
                Name = "Field value comparison"
            });

            // Validations
            defaultFunctions.Add(new WorkflowFunction()
            {
                Id = ObjectId.Parse("5aa805de0af6814a103b25b0"),
                Type = WorkflowFunctionTypes.Validation,
                Name = "Field value required"
            });
            defaultFunctions.Add(new WorkflowFunction()
            {
                Id = ObjectId.Parse("5aa805df0af6814a103b25b1"),
                Type = WorkflowFunctionTypes.Validation,
                Name = "Field value comparison"
            });

            // Post-Functions
            defaultFunctions.Add(new WorkflowFunction()
            {
                Id = ObjectId.Parse("5aa805df0af6814a103b25b2"),
                Type = WorkflowFunctionTypes.PostFunction,
                Name = "Set field value"
            });
            defaultFunctions.Add(new WorkflowFunction()
            {
                Id = ObjectId.Parse("5aa805e00af6814a103b25b3"),
                Type = WorkflowFunctionTypes.PostFunction,
                Name = "Send email notification"
            });
            defaultFunctions.Add(new WorkflowFunction()
            {
                Id = ObjectId.Parse("5aa805e00af6814a103b25b4"),
                Type = WorkflowFunctionTypes.PostFunction,
                Name = "Save changes to history"
            });
            defaultFunctions.Add(new WorkflowFunction()
            {
                Id = ObjectId.Parse("5aa805e00af6814a103b25b5"),
                Type = WorkflowFunctionTypes.PostFunction,
                Name = "Make API call"
            });
            defaultFunctions.Add(new WorkflowFunction()
            {
                Id = ObjectId.Parse("5aa805e00af6814a103b25b6"),
                Type = WorkflowFunctionTypes.PostFunction,
                Name = "Set Item Status"
            });

            foreach (WorkflowFunction function in defaultFunctions)
            {
                await Add(function);
            }
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
                var results = await _context.WorkflowFunctions.Find(filter).ToListAsync();

                if (results == null || results.Count == 0)
                {
                    await CreateDefaultFunctions();
                }

                return await _context.WorkflowFunctions.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task Add(WorkflowFunction item)
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
