using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meticulos.Api.App.Icons
{
    public interface IIconSearcher
    {
        Task<List<Icon>> SearchForIcons(string term);
    }
}
