using System;
using CounterRule;
using MongoDB.Bson.Serialization.Attributes;

namespace Counter.Entity
{
    public class Document:IMeterDocument
    {
        public  DateTime Start { get; set; }        
        public long EllepsitTime { get; set; }
        [BsonIgnoreIfDefault]
        public byte[] RequestData { get; set; }
        [BsonIgnoreIfDefault]
        public byte[] ResponseData { get; set; }
        [BsonIgnoreIfDefault]
        public int ResponseStutus { get; set; }
        public string Ip { get; set; }
        public string UserName { get; set; }
    }
}
