using System.Collections.Generic;
using System.Linq;

namespace Meticulos.Api.App.Items
{
    public class ItemExpansionParams
    {
        public ItemExpansionParams(string paramsString)
        {
            ParseString(paramsString);
        }

        public bool LinkedItems { get; set; }
        public bool Transitions { get; set; }

        public void ParseString(string paramsString)
        {
            if (!string.IsNullOrEmpty(paramsString))
            {
                List<string> queryParams = paramsString.Split(',')
                    .Select(p => p.ToLower()).ToList();

                LinkedItems = queryParams.Contains("linkeditems");
                Transitions = queryParams.Contains("transitions");
            }
        }
    }
}
