using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Counter.Entity;
using CounterRule;
using Microsoft.AspNetCore.Http;

namespace Counter
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
            document.Start = DateTime.Now;
            string authHeader = context.Request.Headers["Authorization"];
            string url= context.Request.Path.Value.ParseUrl();
            if (authHeader != null && authHeader.StartsWith("Basic "))
            {
                var encodeUser = authHeader.Split(' ')[1]?.Trim();
                if (!string.IsNullOrEmpty(encodeUser))
                {
                   var decodeUserName =  Encoding.UTF8.GetString(Convert.FromBase64String(encodeUser));
                    var username = decodeUserName.Split(':')[0];
                    document.UserName = username;
                }
                           
            }
            var timer=Stopwatch.StartNew();
            await _next.Invoke(context);
            document.EllepsitTime = timer.ElapsedMilliseconds;
            document.ResponseStutus = context.Response.StatusCode;
            
            if (context.Response.StatusCode!=200)
            {
                GetRequest(context, document);
                GetResponse(context, document);
                await _service.ErrorResponse(url,context, document);
            }else  await  _service.Request(url, context,document);
            
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
        public void GetResponse(HttpContext context, Document document)
        {
            byte[] data = new byte[context.Response.Body.Length];
            context.Response.Body.Read(data, 0, data.Length);
            document.ResponseData = data;
        }
        /*
        public async Task Invoke(HttpContext context)
        {
            string authHeader = context.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic "))
            {
                // Get the encoded username and password
                var encodedUsernamePassword = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();
                // Decode from Base64 to string
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                // Split username and password
                var username = decodedUsernamePassword.Split(':', 2)[0];
                var password = decodedUsernamePassword.Split(':', 2)[1];
                // Check if login is correct
                if (await IsAuthorized(username, password))
                {
                    await next1.Invoke(context);
                    return;
                }
            }
            // Return authentication type (causes browser to show login dialog)
            context.Response.Headers["WWW-Authenticate"] = "Basic";
            // Add realm if it is not null
            if (!string.IsNullOrWhiteSpace(realm))
            {
                context.Response.Headers["WWW-Authenticate"] += $" realm=\"{realm}\"";
            }
            // Return unauthorized
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
        // Make your own implementation of this
        public async Task<bool> IsAuthorized(string username, string password)
        {

            var customer = await _customers.GetCustomersUserName(username);
            if (customer == null)
                return false;
            // Check that username and password are correct
            return true;

        }*/
    }

}