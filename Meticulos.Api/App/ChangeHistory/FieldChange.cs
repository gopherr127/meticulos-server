using MongoDB.Bson;

namespace Meticulos.Api.App.ChangeHistory
{
    public class FieldChange
    {
        public ObjectId FieldId { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}