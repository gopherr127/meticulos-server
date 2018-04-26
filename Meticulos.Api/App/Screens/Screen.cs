using Meticulos.Api.App.Fields;
using System.Collections.Generic;

namespace Meticulos.Api.App.Screens
{
    public class Screen : Entity
    {
        public string Name { get; set; }
        public List<FieldMetadata> Fields { get; set; }
    }
}
