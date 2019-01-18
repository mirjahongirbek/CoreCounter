using MongoDB.Bson.Serialization.Attributes;

namespace Counter.Entity
{
    public class ErrorDocument
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public long Start { get; set; }
        public long EndTime { get; set; }
        
        
    }

}
