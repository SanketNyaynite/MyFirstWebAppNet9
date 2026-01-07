var builder = WebApplication.CreateBuilder(args);       //sets up Kestrel server.
/*  Between the above and below lines, we can configure the Kestrel server.
 * 
 * var builder = WebApplication.CreateBuilder(args);
 * 
 * var app = builder.Build(); 
 */
var app = builder.Build();                              //generates the instance of web application.

app.MapGet("/", () => "Hello World!");                  //corresponds to the middleware component and processes HTTP requests.Lamba function

app.Run();                                              //runs the web application as well as listens to kestrel server.
