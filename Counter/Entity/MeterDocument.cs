using System.Collections.Generic;
using CounterRule;
using MongoDB.Bson.Serialization.Attributes;

namespace Counter.Entity
{
    public class MeterDocument:IMeter<Document>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public long Start {get; set;} 
        public long End { get; set; }
        public string MethodUrl { get; set; }
        public List<Document> Request { get; set; }
        public int RequestCount { get; set; }
    }

}
