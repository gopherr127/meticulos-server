using System;
using MongoDB.Bson;

namespace Meticulos.Api.App.FieldOptions
{
    public class FieldOption : Entity, IEquatable<FieldOption>
    {
        public ObjectId FieldId { get; set; }
        public string Name { get; set; }

        public bool Equals(FieldOption other)
        {
            return other.Id == Id 
                && other.Name == Name
                && other.FieldId == FieldId;
        }
    }
}
