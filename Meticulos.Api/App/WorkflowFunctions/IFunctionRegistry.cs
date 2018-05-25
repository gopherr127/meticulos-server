using System.Collections.Generic;
using System.Threading.Tasks;
using Meticulos.Api.App.Items;
using Meticulos.Api.App.WorkflowTransitionFunctions;

namespace Meticulos.Api.App.WorkflowFunctions
{
    public interface IFunctionRegistry
    {
        List<WorkflowFunction> GetDefaultFunctions();
        Task<OperationResult<bool>> ExecuteFunction(
            WorkflowTransitionFunction transitionFunction, Item item);
    }
}