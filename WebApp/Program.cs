var builder = WebApplication.CreateBuilder(args);       //sets up Kestrel server.
/*  Between the above and below lines, we can configure the Kestrel server.
 * 
 * var builder = WebApplication.CreateBuilder(args);
 * 
 * var app = builder.Build(); 
 */
var app = builder.Build();                              //generates the instance of web application.

//Below is a Http request Run method that handles all HTTP requests.
app.Run(async (HttpContext context) =>                 //corresponds to the middleware component and processes HTTP requests.Lamba function
{
    await context.Response.WriteAsync($"The method is: {context.Request.Method}\r\n");
    await context.Response.WriteAsync($"The url is: {context.Request.Path}\r\n");

    await context.Response.WriteAsync($"\r\nHeaders:\r\n");
    foreach (var key in context.Request.Headers.Keys)
    {
       await context.Response.WriteAsync($"{key}: {context.Request.Headers[key]}\r\n");
    }
});
app.Run();                                              //runs the web application as well as listens to kestrel server.


    //WriteAsync is an asynchronous method that writes the specified string to the HTTP response body. So basically it is going to output data to response object and then when the reponse is sent to the browser, the browser is able to display that data.