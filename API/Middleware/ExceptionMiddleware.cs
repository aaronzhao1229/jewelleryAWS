using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env) 
        // RequestDelegate allows us to execute the next method and pass on the request to the next piece of middleware.
        //ILogger to log out the exception
        // IHostEnvironment to check if we're running in production mode or development mode
        {
            _env = env;
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context) // it has to be the name of InvokeAsync because the framework is expectin our middleware to have this particular method
        {
            try
            {
                await _next(context);   // call the next piece of middleware
            }
            catch (Exception ex)
            {
              
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json"; // access the context. We need to specify the content type here because we're not inside the context of an api controller.
                context.Response.StatusCode = 500;

                var response = new ProblemDetails // create a response which retains the same format as the rest of our errors in our application
                {
                    Status = 500,
                    Detail = _env.IsDevelopment() ? ex.StackTrace.ToString() : null, // we don't want to cause an exception within our catch block really. So just in case the stac trace for whatever reason is null, then we will use optional chaining here before we execute the toString() method. 
                    Title = ex.Message
                };

                // create some options for the JSON serializor. Because we're not inside the context of an api controller, we lose some defaults that that does for us. When our API controller returns a JSON response, then it uses camel case. But outside of an API controller, we need to specify these options.
                var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json); //what we are going to return to the client if we get that exception
            }
        }
    }
}