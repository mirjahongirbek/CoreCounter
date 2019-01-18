using System.Collections.Generic;

namespace CounterRule
{
    public interface IMeter<T> where T:class , IMeterDocument
    {
         string Id { get; set; }
         long Start { get; set; }
         long End { get; set; }
         string MethodUrl { get; set; }
         List<T> Request { get; set; }
         int RequestCount { get; set; }
    }
    
}
