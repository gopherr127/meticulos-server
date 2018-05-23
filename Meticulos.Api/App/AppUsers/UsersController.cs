using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Meticulos.Api.App.AppUsers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IAppUserRepository _appUserRepository;
        private AuthTokenResponse _authTokenInfo;

        public UsersController(IAppUserRepository appUserRepository)
        {
            _appUserRepository = appUserRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {
                if (_authTokenInfo == null)
                {
                    await GetAuthToken();
                }

                var getUsersResponse = await GetUsers(_authTokenInfo);

                if (!getUsersResponse.IsSuccessStatusCode)
                {   // A one-time retry
                    var authTokenInfo = await GetAuthToken();

                    getUsersResponse = await GetUsers(_authTokenInfo);
                }

                var responseString = await getUsersResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<GetUsersResponse>(responseString);
                return result.users;
            });
        }

        private async Task<HttpResponseMessage> GetAuthToken()
        {
            AuthTokenRequestBody body = new AuthTokenRequestBody()
            {
                grant_type = "client_credentials",
                client_id = "gKSmnV0oEH5w9zSL42LsnOifEOG6u1N0",
                client_secret = "jzrBNtFSVwy00zMc12QNqvCxLZE2cB2rsfnnXvGZK6BwmR6fXQleYB84AgxpPk1q",
                audience = "urn:auth0-authz-api"
            };

            using (var client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://atlasrfid.auth0.com/oauth/token");
                request.Content = new StringContent(JsonConvert.SerializeObject(body), System.Text.Encoding.UTF8, "application/json");
                var response = await client.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();
                _authTokenInfo = JsonConvert.DeserializeObject<AuthTokenResponse>(responseString);

                return response;
            }
        }

        private async Task<HttpResponseMessage> GetUsers(AuthTokenResponse tokenResponse)
        {
            using (var client = new HttpClient())
            {
                // Header for subsequent request to get users
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(tokenResponse.token_type, tokenResponse.access_token);

                var httpResponse = await client.GetAsync("https://atlasrfid.us.webtask.io/adf6e2f2b84784b57522e3b19dfc9201/api/users");

                return httpResponse;
            }
        }

        //[Route("{id:guid}")]
        //[HttpGet]
        //public async Task<IActionResult> Get(Guid id)
        //{
        //    return await FunctionWrapper.ExecuteFunction(this, async () =>
        //    {

        //    });
        //}

        //[Route("authenticate")]
        //[HttpGet]
        //public async Task<IActionResult> Get(string emailAddress, string password)
        //{
        //    return await FunctionWrapper.ExecuteFunction(this, async () =>
        //    {
        //        return await _appUserRepository.Authenticate(new AppUser()
        //        {
        //            EmailAddress = emailAddress,
        //            Password = password
        //        });
        //    });
        //}

        //[Route("{id:length(24)}")]
        //[HttpGet]
        //public async Task<IActionResult> Get(string id)
        //{
        //    return await FunctionWrapper.ExecuteFunction(this, async () =>
        //    {
        //        return await _appUserRepository.Get(new ObjectId(id));
        //    });
        //}

        //[HttpPost]
        //public async Task<IActionResult> Post([FromBody]AppUser item)
        //{
        //    return await FunctionWrapper.ExecuteFunction(this, async () =>
        //    {
        //        return await _appUserRepository.Add(item);
        //    });
        //}

        //[Route("{id:length(24)}")]
        //[HttpPut]
        //public async Task<IActionResult> Put(string id, [FromBody]AppUser item)
        //{
        //    return await FunctionWrapper.ExecuteFunction(this, async () =>
        //    {
        //        item.Id = new ObjectId(id);
        //        return await _appUserRepository.Update(item);
        //    });
        //}

        //[Route("{id:length(24)}")]
        //[HttpDelete]
        //public async Task<IActionResult> Delete(string id)
        //{
        //    return await FunctionWrapper.ExecuteAction(this, async () =>
        //    {
        //        await _appUserRepository.Delete(new ObjectId(id));
        //    });
        //}
    }

    public class AuthTokenRequestBody
    {
        public string grant_type { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string audience { get; set; }
    }

    public class AuthTokenResponse
    {
        public string access_token { get; set; }
        public string scope { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
    }

    public class GetUsersResponse
    {
        public List<UserObject> users { get; set; }
    }

    public class UserObject
    {
        public string email { get; set; }
        public string name { get; set; }
        public string last_login { get; set; }
    }
}
