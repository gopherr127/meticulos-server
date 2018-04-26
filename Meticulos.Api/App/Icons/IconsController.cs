using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.Icons
{
    [Route("api/[controller]")]
    public class IconsController : Controller
    {
        private readonly IIconSearcher _iconSearcher;

        public IconsController(IServiceProvider serviceProvider)
        {
            _iconSearcher = (IIconSearcher)serviceProvider.GetService(typeof(IIconSearcher));
        }

        [Route("search")]
        [HttpGet]
        public async Task<List<Icon>> Search([FromQuery]string term)
        {
            return await _iconSearcher.SearchForIcons(term);
        }

    }
}
