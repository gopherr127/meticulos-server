using System.Threading.Tasks;

namespace Meticulos.Api.App.WorkflowFunctions
{
    public interface IWorkflowFunction
    {
        string Name { get; set; }
        WorkflowFunctionTypes Type { get; set; }

        Task<OperationResult<bool>> Execute(string argsObject);
    }
}