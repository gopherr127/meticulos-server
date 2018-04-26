using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.ChangeHistory
{
    [Route("api/[controller]")]
    public class FieldChangeGroupsController : Controller
    {
        private readonly IFieldChangeGroupRepository _fieldChangeGroupRepository;

        public FieldChangeGroupsController(IFieldChangeGroupRepository fieldChangeGroupRepository)
        {
            _fieldChangeGroupRepository = fieldChangeGroupRepository;
        }
        
        [Route("search")]
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery]FieldChangeGroupSearchRequest requestArgs)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                if (string.IsNullOrEmpty(requestArgs.ItemId)
                || ObjectId.Parse(requestArgs.ItemId) == ObjectId.Empty)
                {
                    throw new ApplicationException("Search parameters missing or invalid.");
                }

                return await _fieldChangeGroupRepository.Search(new ObjectId(requestArgs.ItemId));
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]FieldChangeGroup item)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () => {

                if (item.ItemId == ObjectId.Empty)
                {
                    throw new ApplicationException("Required fields not supplied.");
                }

                return await _fieldChangeGroupRepository.Add(item);
            });
        }

    }
}
