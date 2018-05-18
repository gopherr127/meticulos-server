using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace Meticulos.Api.App.AppUsers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IAppUserRepository _appUserRepository;

        public UsersController(IAppUserRepository appUserRepository)
        {
            _appUserRepository = appUserRepository;
        }

        [Route("authenticate")]
        [HttpGet]
        public async Task<IActionResult> Get(string emailAddress, string password)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {
                return await _appUserRepository.Authenticate(new AppUser()
                {
                    EmailAddress = emailAddress,
                    Password = password
                });
            });
        }

        [Route("{id:length(24)}")]
        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {
                return await _appUserRepository.Get(new ObjectId(id));
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]AppUser item)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {
                return await _appUserRepository.Add(item);
            });
        }

        [Route("{id:length(24)}")]
        [HttpPut]
        public async Task<IActionResult> Put(string id, [FromBody]AppUser item)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {
                item.Id = new ObjectId(id);
                return await _appUserRepository.Update(item);
            });
        }

        [Route("{id:length(24)}")]
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            return await FunctionWrapper.ExecuteAction(this, async () =>
            {
                await _appUserRepository.Delete(new ObjectId(id));
            });
        }
    }
}
