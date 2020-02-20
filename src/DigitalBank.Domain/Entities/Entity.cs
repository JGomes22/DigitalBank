using DigitalBank.Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace DigitalBank.Domain.Entities
{
    public abstract class Entity
    {
        public Entity() => ValidationErrors = new List<Error>();

        [BsonId]
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public abstract bool IsValid(EValidationStage eValidationStage);
        [BsonIgnore]
        public IList<Error> ValidationErrors { get; set; }


    }
}
