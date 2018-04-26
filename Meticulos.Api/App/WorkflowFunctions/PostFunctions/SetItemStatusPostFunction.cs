using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Meticulos.Api.App.WorkflowFunctions.PostFunctions
{
    public class SetItemStatusPostFunction : WorkflowFunction, IWorkflowFunction
    {
        public Task<OperationResult<bool>> Execute(string argsObject)
        {
            return Task<OperationResult<bool>>.Factory.StartNew(() =>
            {
                var args = JsonConvert.DeserializeObject<SetItemStatusPostFunctionArgs>(argsObject);

                // ... 

                return new OperationResult<bool>() { Value = true };
            });
        }
    }

    public class SetItemStatusPostFunctionArgs
    {

    }
}
