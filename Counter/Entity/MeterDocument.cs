using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace Counter.Entity
{
    public class MeterDocument
    {
        [BsonId]
        public string Id { get; set; }
        public DateTime Start {get; set;} 
        public DateTime End { get; set; }
        public string MethodUrl { get; set; }
        public List<Document> Request { get; set; }
    }
    public class Document
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
    public static class CacheData
    {
        public static Dictionary<string, Data> Data = new Dictionary<string, Data>();
        public static string ParseUrl(this string url)
        {

            return url;
        }
    };
    public class Data
    {
        public string Id { get; set; }
        public int EndTime { get; set; }
        public int Count { get; set; }
    }
}
