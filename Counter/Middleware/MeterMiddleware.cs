using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Counter.Entity;
using CounterRule;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Counter.Middleware
{
    public class MeterMiddleware
    {
        private readonly RequestDelegate _next;
        IMeterService<MeterDocument, Document> _service;
        public MeterMiddleware(RequestDelegate next, IMeterService<MeterDocument, Document> service)
        {
            this._next = next;
            _service = service;
                
        }
        public async Task InvokeAsync(HttpContext context)
        {
            Document document = new Document();
            document.Start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string authHeader = context.Request.Headers["Authorization"];
            string url= context.Request.Path.Value.ParseUrl();
            Stream orginalBody = context.Response.Body;




            try
            {
                using (var memStream = new MemoryStream()){
                    context.Response.Body = memStream;

                    var timer = Stopwatch.StartNew();
                    await _next(context);
                    document.EllepsitTime = timer.ElapsedMilliseconds;
                    memStream.Position = 0;
                    await memStream.CopyToAsync(orginalBody);
                    document.ResponseStutus = context.Response.StatusCode;
                    
                    if (context.Response.StatusCode != 200)
                    {
                        GetRequest(context, document);
                        GetResponse(memStream, document);
                        
                        Task.Run(async () =>
                        {
                            await _service.ErrorResponse(url, context, document);
                        });
                    }
                    else Task.Run(async () => { await _service.Request(url, context, document); });
                    //old


                }
            }
            finally
            {
                context.Response.Body = orginalBody;
            }
                        
        }
        public void GetRequest(HttpContext context, Document document)
        {
            if (!string.IsNullOrEmpty(context.Request.QueryString.Value))
            {
                document.RequestData = Encoding.ASCII.GetBytes(context.Request.QueryString.Value);
                return;
            }
            byte[] data = new byte[(int)context.Request.ContentLength];
            if (data.Length == 0) return;

            context.Request.Body.Read(data, 0, data.Length);
            document.RequestData = data;
        }
        public void GetResponse(MemoryStream memStream, Document document)
        {
            memStream.Position = 0;
            byte[] data = new byte[memStream.Length];
            memStream.Read(data, 0, data.Length);
            document.ResponseData = data;
        }
        

    }

}