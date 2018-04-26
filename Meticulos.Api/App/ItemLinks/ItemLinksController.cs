using Meticulos.Api.App.Items;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.ItemLinks
{
    [Route("api/[controller]")]
    public class ItemLinksController : Controller
    {
        private readonly IItemRepository _itemRepository;

        public ItemLinksController(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        [HttpPost]
        public async Task Post([FromBody]ItemLink link)
        {
            
        }

        [HttpDelete]
        public async Task Delete([FromBody]ItemLink link)
        {
            
        }
    }
}
