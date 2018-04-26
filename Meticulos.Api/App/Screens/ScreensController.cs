using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.Screens
{
    [Route("api/[controller]")]
    public class ScreensController : Controller
    {
        private readonly IScreenRepository _screenRepository;

        public ScreensController(IScreenRepository workflowRepository)
        {
            _screenRepository = workflowRepository;
        }

        [Route("{id:length(24)}")]
        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                return await _screenRepository.Get(new ObjectId(id));
            });
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                return await _screenRepository.GetAll();
            });
        }

        [Route("find")]
        [HttpPost]
        public async Task<IActionResult> Find([FromBody]List<ObjectId> screenIds)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                return await _screenRepository.Find(screenIds);
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Screen screen)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                if (string.IsNullOrEmpty(screen.Name))
                {
                    throw new System.Exception("Required fields not supplied.");
                }

                return await _screenRepository.Add(screen);
            });
        }

        [Route("{id:length(24)}")]
        [HttpPut]
        public async Task<IActionResult> Put(string id, [FromBody]Screen screen)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                if (string.IsNullOrEmpty(screen.Name))
                {
                    throw new System.Exception("Required fields not supplied.");
                }

                screen.Id = new ObjectId(id);
                return await _screenRepository.Update(screen);
            });
        }

        [Route("{id:length(24)}")]
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            return await FunctionWrapper.ExecuteAction(this, async () =>
            {

                await _screenRepository.Delete(new ObjectId(id));
            });
        }
    }
}
