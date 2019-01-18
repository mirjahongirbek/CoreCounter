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
    };
}
