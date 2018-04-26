using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.WorkflowFunctions.PreConditions
{
    public class UserInGroupPreCondition : WorkflowFunction, IWorkflowFunction
    {
        public Task<OperationResult<bool>> Execute(string argsObject)
        {
            return Task.Run(() => {

                UserInGroupPreConditionArgs args =
                    JsonConvert.DeserializeObject<UserInGroupPreConditionArgs>(argsObject);

                //TODO: Obviously, need to do something meaningful here...
                if (args.GroupIds.Count % 2 == 0)
                    return new OperationResult<bool>() { Value = true };

                return new OperationResult<bool>()
                {
                    ErrorMessage = "User is not in a required group."
                };
            });
        }
    }

    public class UserInGroupPreConditionArgs
    {
        public List<string> GroupIds { get; set; }
    }
}
