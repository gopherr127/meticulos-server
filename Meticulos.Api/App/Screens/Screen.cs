using Meticulos.Api.App.Fields;
using System.Collections.Generic;

namespace Meticulos.Api.App.Screens
{
    public class Screen : Entity
    {
        public string Name { get; set; }
        public List<FieldMetadata> Fields { get; set; }
        public bool DisplayLocation { get; set; }
        public bool DisplayLinkedItems { get; set; }
        public bool DisplayChildItems { get; set; }
        public bool DisplayImageCapture { get; set; }
        public bool DisplayAttachments { get; set; }
    }
}
