using MongoDB.Bson;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.WorkflowFunctions.PreConditions
{
    public class UserInRolePreCondition : WorkflowFunction, IWorkflowFunction
    {
        public Task<OperationResult<bool>> Execute(string argsObject)
        {
            return Task<OperationResult<bool>>.Factory.StartNew(() => {

                UserInRolePreConditionArgs args =
                    JsonConvert.DeserializeObject<UserInRolePreConditionArgs>(argsObject);

                //TODO: Obviously, need to do something meaningful here...
                if (args.RoleIds.Count % 2 == 0)
                    return new OperationResult<bool>() { Value = true };

                return new OperationResult<bool>() {
                    ErrorMessage = "User does not have a required role."
                };
            });
        }
    }

    public class UserInRolePreConditionArgs
    {
        public List<string> RoleIds { get; set; }
    }
}
