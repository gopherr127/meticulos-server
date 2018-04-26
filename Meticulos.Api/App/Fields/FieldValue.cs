using MongoDB.Bson;

namespace Meticulos.Api.App.Fields
{
    public class FieldValue
    {
        public ObjectId FieldId { get; set; }
        public string FieldName { get; set; }
        public string Value { get; set; }
    }
}
