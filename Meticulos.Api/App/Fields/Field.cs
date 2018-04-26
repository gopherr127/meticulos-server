using Meticulos.Api.App.FieldOptions;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Meticulos.Api.App.Fields
{
    public class Field : Entity
    {
        public string Name { get; set; }
        public FieldTypes Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DefaultValue { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<FieldOption> ValueOptions { get; set; }
    }
}
