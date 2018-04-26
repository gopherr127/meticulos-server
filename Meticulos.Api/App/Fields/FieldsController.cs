using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.Fields
{
    [Route("api/[controller]")]
    public class FieldsController : Controller
    {
        private readonly IFieldRepository _fieldRepository;

        public FieldsController(IFieldRepository workflowRepository)
        {
            _fieldRepository = workflowRepository;
        }

        [Route("{id:length(24)}")]
        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                return await _fieldRepository.Get(new ObjectId(id));
            });
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                return await _fieldRepository.GetAll();
            });
        }

        [Route("find")]
        [HttpPost]
        public async Task<IActionResult> Find([FromBody]List<ObjectId> fieldIds)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                return await _fieldRepository.Find(fieldIds);
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Field field)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                if (string.IsNullOrEmpty(field.Name))
                {
                    throw new System.Exception("Required fields not supplied.");
                }

                return await _fieldRepository.Add(field);
            });
        }

        [Route("{id:length(24)}")]
        [HttpPut]
        public async Task<IActionResult> Put(string id, [FromBody]Field field)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                if (string.IsNullOrEmpty(field.Name))
                {
                    throw new System.Exception("Required fields not supplied.");
                }

                field.Id = new ObjectId(id);
                return await _fieldRepository.Update(field);
            });
        }

        [Route("{id:length(24)}")]
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            return await FunctionWrapper.ExecuteAction(this, async () =>
            {

                await _fieldRepository.Delete(new ObjectId(id));
            });
        }
    }
}
