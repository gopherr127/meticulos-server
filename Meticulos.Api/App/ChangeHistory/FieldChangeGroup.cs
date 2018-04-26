using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace Meticulos.Api.App.ChangeHistory
{
    public class FieldChangeGroup : Entity
    {
        public ObjectId ItemId { get; set; }
        public ObjectId ChangedByUserId { get; set; }
        public DateTime ChangedDateTime { get; set; }
        public List<FieldChange> FieldChanges { get; set; }
    }
}
