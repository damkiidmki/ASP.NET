using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Pcf.Administration.Core.Domain;

public abstract class BaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; } = Guid.NewGuid();
}