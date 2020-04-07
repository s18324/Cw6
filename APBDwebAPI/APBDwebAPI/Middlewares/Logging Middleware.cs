using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APBDwebAPI.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();
            string method = httpContext.Request.Method;
            string path = httpContext.Request.Path.ToString();
            string query = httpContext.Request.QueryString.ToString();

            string body = "";
            using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
            {

                body = await reader.ReadToEndAsync();
                httpContext.Request.Body.Position = 0;
            }


            using (var wr = File.AppendText(@"file.txt"))
            {
                wr.WriteLine($"method = {method}");
                wr.WriteLine($"path = {path}");
                wr.WriteLine($"Query = {query}");
                wr.WriteLine(body);
            }
            await _next(httpContext);
        }

    }
}
