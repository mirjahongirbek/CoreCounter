using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CounterRule
{
    public interface IMeterService<T, TDocument> 
        where T :class, IMeter where TDocument:class, IMeterDocument
    {
        Task Request(string url,HttpContext context, TDocument document);
        Task ErrorResponse(string url, HttpContext context, TDocument document);

    }
    public interface IMeterDocument
    {

    }
    public interface IMeter
    {

    }
    
}
