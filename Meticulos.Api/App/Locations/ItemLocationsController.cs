using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Threading.Tasks;


namespace Meticulos.Api.App.Locations
{
    [Route("api/[controller]")]
    public class ItemLocationsController : Controller
    {
        private readonly IItemLocationRepository _itemLocationRepository;

        public ItemLocationsController(IItemLocationRepository itemLocationRepository)
        {
            _itemLocationRepository = itemLocationRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                return await _itemLocationRepository.GetAll();
            });
        }

        [Route("{id:length(24)}")]
        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                return await _itemLocationRepository.Get(new ObjectId(id));
            });
        }

        //[Route("search")]
        //[HttpGet]
        //public async Task<IActionResult> Search([FromQuery]ItemLocationSearchRequest requestArgs)
        //{
        //    return await FunctionWrapper.ExecuteFunction(this, async () =>
        //    {

        //        if (string.IsNullOrEmpty(requestArgs.WorkflowId))
        //        {
        //            throw new System.Exception("Invalid search parameters supplied.");
        //        }

        //        return await _itemLocationRepository.Search(new ObjectId(requestArgs.WorkflowId));
        //    });
        //}

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ItemLocation item)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                return await _itemLocationRepository.Add(item);
            });
        }

        [Route("{id:length(24)}")]
        [HttpPut]
        public async Task<IActionResult> Put(string id, [FromBody]ItemLocation item)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                item.Id = new ObjectId(id);
                return await _itemLocationRepository.Update(item);
            });
        }

        [Route("{id:length(24)}")]
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            return await FunctionWrapper.ExecuteAction(this, async () =>
            {

                await _itemLocationRepository.Delete(new ObjectId(id));
            });
        }
    }
}
