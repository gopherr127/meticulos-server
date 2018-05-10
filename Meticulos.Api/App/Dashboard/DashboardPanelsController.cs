using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace Meticulos.Api.App.Dashboard
{
    [Route("api/[controller]")]
    public class DashboardPanelsController : Controller
    {
        private readonly IDashboardPanelRepository _dashboardPanelRepository;

        public DashboardPanelsController(IDashboardPanelRepository dashboardPanelRepository)
        {
            _dashboardPanelRepository = dashboardPanelRepository;
        }

        [Route("{id:length(24)}")]
        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                return await _dashboardPanelRepository.Get(new ObjectId(id));
            });
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                return await _dashboardPanelRepository.GetAll();
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]DashboardPanel item)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                if (string.IsNullOrEmpty(item.Title))
                {
                    throw new System.Exception("Required fields not supplied.");
                }

                return await _dashboardPanelRepository.Add(item);
            });
        }

        [Route("{id:length(24)}")]
        [HttpPut]
        public async Task<IActionResult> Put(string id, [FromBody]DashboardPanel item)
        {
            return await FunctionWrapper.ExecuteFunction(this, async () =>
            {

                if (string.IsNullOrEmpty(item.Title))
                {
                    throw new System.Exception("Required fields not supplied.");
                }

                item.Id = new ObjectId(id);
                return await _dashboardPanelRepository.Update(item);
            });
        }

        [Route("{id:length(24)}")]
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            return await FunctionWrapper.ExecuteAction(this, async () =>
            {

                await _dashboardPanelRepository.Delete(new ObjectId(id));
            });
        }
    }
}
