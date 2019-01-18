using System;
using System.Collections.Generic;

namespace Counter.Entity
{
    public static class CacheData
    {
        public static Dictionary<string, Data> Data = new Dictionary<string, Data>();
        public static string ParseUrl(this string url)
        {
            return url;
        }
        public static ErrorBase ErrorBase { get; set; }
        public static Dictionary<string, List<Document>> ErrorCache = new Dictionary<string, List<Document>>();
        public static long Now
        {
            get { return DateTimeOffset.UtcNow.ToUnixTimeSeconds(); }
        }
        
    };
    public class ErrorBase
    {
        public string Id { get; set; }
        public long EndTime { get; set; }
        public long Start { get; set; }
    } 
    public class ErrorData
    {
        public ErrorData()
        {
            Documents = new List<Document>();
        }
        
        public List<Document> Documents { get; set; }
        
    }
    
}
