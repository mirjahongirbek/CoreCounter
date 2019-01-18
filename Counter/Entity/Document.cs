using System;
using CounterRule;
using MongoDB.Bson.Serialization.Attributes;

namespace Counter.Entity
{
    public class Document:IMeterDocument
    {
        
        public  long Start { get; set; }        
        public long EllepsitTime { get; set; }
        [BsonIgnoreIfDefault]
        public byte[] RequestData { get; set; }
        [BsonIgnoreIfDefault]
        public byte[] ResponseData { get; set; }
        [BsonDefaultValue(200)]
        [BsonIgnoreIfDefault]
        public int ResponseStutus { get; set; }
        [BsonIgnoreIfNull]
        public string Ip { get; set; }
        [BsonIgnoreIfNull]
        public string UserName { get; set; }
    }
}
