using System.Text.Json;
using static EmployeesRepository;

var builder = WebApplication.CreateBuilder(args);       //sets up Kestrel server.
/*  Between the above and below lines, we can configure the Kestrel server.
 * 
 * var builder = WebApplication.CreateBuilder(args);
 * 
 * var app = builder.Build(); 
 */
var app = builder.Build();                              //generates the instance of web application.

//Below is a Http request Run method that handles all HTTP requests.
//Below corresponds to the middleware component and processes HTTP requests.Lamba function.
app.Run(async (HttpContext context) =>                 
{
    if (context.Request.Method == "GET")                
    {
        if (context.Request.Path.StartsWithSegments("/"))
        {
            await context.Response.WriteAsync($"The method is: {context.Request.Method}\r\n");
            await context.Response.WriteAsync($"The url is: {context.Request.Path}\r\n");

            await context.Response.WriteAsync($"\r\nHeaders:\r\n");
            foreach (var key in context.Request.Headers.Keys)
            {
                await context.Response.WriteAsync($"{key}: {context.Request.Headers[key]}\r\n");
            }
        }
        else if (context.Request.Path.StartsWithSegments("/employees"))
        {
            //await context.Response.WriteAsync("Employee List");
            var employees = EmployeesRepository.GetAllEmployees();

            foreach (var employee in employees)
            {
                await context.Response.WriteAsync($"{employee.Name}: {employee.Position}\r\n");
            }
        }
        else
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("404 Not Found");
        }
    }
    else if (context.Request.Method == "POST")
    {
        if (context.Request.Path.StartsWithSegments("/employees"))
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            var employee = JsonSerializer.Deserialize<Employee>(body);

            EmployeesRepository.AddEmployee(employee);

        }
        else
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("404 Not Found");
        }
    }
    else
    {
        context.Response.StatusCode = 405; // Method Not Allowed
        await context.Response.WriteAsync("405 Method Not Allowed");
    }
});

app.Run();                                              //runs the web application as well as listens to kestrel server.

static class EmployeesRepository
{
    private static List<Employee> employees = new List<Employee>
    {
            new Employee(1, "John Doe", "Engineer", 60000),
            new Employee(2, "Jane Smith", "Manager", 75000),
            new Employee(3, "Sam Brown", "Technician", 50000)
    };

    public static List<Employee> GetAllEmployees() => employees;
    public static void AddEmployee(Employee? employee)
    {
        if (employee is not null) 
        {
            employees.Add(employee);
        }
    }
}
public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Position { get; set; }
    public double Salary { get; set; }

    public Employee(int id, string name, string position, double salary)
    {
        Id = id;
        Name = name;
        Position = position;
        Salary = salary;
    }
}


/*
 * WriteAsync is an asynchronous method that writes the specified string to the HTTP response body. So basically it is going to output data to response object and then when the reponse is sent to the browser, the browser is able to display that data.
 * 
 * The purpose of GET methid is to retrieve data from the server.
 * The purpose of Http Post method is to create new resource on the server.
 */

