using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;

namespace Meticulos.Api.App.WorkflowFunctions.PostFunctions
{
    public class MakeApiCallPostFunction : WorkflowFunction, IWorkflowFunction
    {
        public async Task<OperationResult<bool>> Execute(string argsObject)
        {
            var args = JsonConvert.DeserializeObject<MakeApiCallPostFunctionArgs>(argsObject);

            using (var client = new HttpClient())
            {
                //TODO: Support adding authorization header for API call
                //client.DefaultRequestHeaders.Authorization = ...

                var httpResponse = await client.PostAsync(args.Url, new StringContent(args.Payload));

                if (httpResponse.IsSuccessStatusCode)
                {
                    return new OperationResult<bool>() { Value = true };
                }
                else
                {
                    return new OperationResult<bool>() { ErrorMessage = httpResponse.ReasonPhrase };
                }
            }
        }
    }

    public class MakeApiCallPostFunctionArgs
    {
        public string Url { get; set; }
        public bool IncludePayload { get; set; }
        public string Payload { get; set; }
    }
}
