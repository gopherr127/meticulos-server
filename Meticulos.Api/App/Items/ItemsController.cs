using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Threading.Tasks;

namespace Meticulos.Api.App.Items
{
    [Route("api/[controller]")]
    public class ItemsController : Controller
    {
        private readonly IItemRepository _itemRepository;

        public ItemsController(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                return await _itemRepository.GetAll();
            });
        }

        [Route("{id:length(24)}")]
        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                return await _itemRepository.Get(new ObjectId(id));
            });
        }

        [Route("search")]
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery]ItemSearchRequest requestArgs)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                if (!string.IsNullOrEmpty(requestArgs.Name))
                {
                    return await _itemRepository.Search(requestArgs.Name);
                }
                else if (!string.IsNullOrEmpty(requestArgs.ParentId))
                {
                    return await _itemRepository.Search(new ObjectId(requestArgs.ParentId));
                }

                return null;
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Item item)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () => {

                if (string.IsNullOrEmpty(item.Name)
                    || item.TypeId == ObjectId.Empty)
                {
                    throw new ApplicationException("Required fields not supplied.");
                }

                return await _itemRepository.Add(item);
            });
        }

        [Route("{id:length(24)}")]
        [HttpPut]
        public async Task<IActionResult> Put(string id, [FromBody]Item item)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                item.Id = new ObjectId(id);
                return await _itemRepository.Update(item);
            });
        }

        [Route("{id:length(24)}")]
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            return await FunctionWrapper.ExecuteAction(this, async () =>
            {

                await _itemRepository.Delete(new ObjectId(id));
            });
        }
    }
}
