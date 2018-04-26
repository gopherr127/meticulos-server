using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.ItemTypes
{
    [Route("api/[controller]")]
    public class ItemTypesController : Controller
    {
        private readonly IItemTypeRepository _itemTypeRepository;

        public ItemTypesController(IItemTypeRepository itemTypeRepository)
        {
            _itemTypeRepository = itemTypeRepository;
        }

        [Route("{id:length(24)}")]
        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                return await _itemTypeRepository.Get(new ObjectId(id));
            });
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                return await _itemTypeRepository.GetAll();
            });
        }

        [Route("search")]
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery]ItemTypeSearchRequest request)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                return await _itemTypeRepository.Search(request);
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ItemType item)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                return await _itemTypeRepository.Add(item);
            });
        }

        [Route("{id:length(24)}")]
        [HttpPut]
        public async Task<IActionResult> Put(string id, [FromBody]ItemType item)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                item.Id = new ObjectId(id);
                return await _itemTypeRepository.Update(item);
            });
        }

        [Route("{id:length(24)}")]
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            return await FunctionWrapper.ExecuteAction(this, async () =>
            {

                await _itemTypeRepository.Delete(new ObjectId(id));
            });
        }
    }
}
