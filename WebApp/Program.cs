using System.Text.Json;
using static EmployeesRepository;

//sets up Kestrel server.
var builder = WebApplication.CreateBuilder(args);
/*  Between the above and below lines, we can configure the Kestrel server.
 * 
 * var builder = WebApplication.CreateBuilder(args);
 * 
 * var app = builder.Build(); 
 */
//generates the instance of web application.
var app = builder.Build();                              

//Below is a Http request Run method that handles all HTTP requests.
//Below corresponds to the middleware component and processes HTTP requests.Lamba function.
app.Run(async (HttpContext context) =>                 
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
        if (context.Request.Method == "GET")
        {
            //keyboard shortcut press k+f to format the code in visual studio.
            var employees = EmployeesRepository.GetAllEmployees();

            foreach (var employee in employees)
            {
                await context.Response.WriteAsync($"{employee.Name}: {employee.Position}\r\n");
            }
        }
        else if (context.Request.Method == "POST")
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            var employee = JsonSerializer.Deserialize<Employee>(body);

            EmployeesRepository.AddEmployee(employee);
        }
        else if (context.Request.Method == "PUT")
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            var employee = JsonSerializer.Deserialize<Employee>(body);

            var result = EmployeesRepository.UpdateEmployee(employee);
            if (result)
            {
                await context.Response.WriteAsync("Employee updated successfully.");
            }
            else
            {
                await context.Response.WriteAsync("Employee not found.");
            }
        }
        else if (context.Request.Method == "DELETE")
        {
            if (context.Request.Query.ContainsKey("id"))
            {
                var id = context.Request.Query["id"];
                if (int.TryParse(id, out int employeeId))
                {
                    if (context.Request.Headers["Authorization"] == "ethan")
                    {
                        var result = EmployeesRepository.DeleteEmployee(employeeId);

                        if (result)
                        {
                            await context.Response.WriteAsync("Employee is deleted successfully.");
                        }
                        else
                        {
                            await context.Response.WriteAsync("Employee not found.");
                        }
                    }
                    else
                    {
                        await context.Response.WriteAsync("You are not authorized to delete.");

                    }
                }
            }
        }
    }

        //Below code is used to read query strings from the URL.
        //foreach (var key in context.Request.Query.Keys)
        //{
        //    await context.Response.WriteAsync($"{key}: {context.Request.Query[key]}\r\n");
        //}
    });
//runs the web application as well as listens to kestrel server.
app.Run();                                              

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

    public static Boolean UpdateEmployee(Employee? employee)
    {
        if (employee is not null)
        {
            var emp = employees.FirstOrDefault(x => x.Id == employee.Id);
            if (emp != null)
            {
                emp.Name = employee.Name;
                emp.Position = employee.Position;
                emp.Salary = employee.Salary;
                return true;
            }
        }
        return false;
    }

    public static Boolean DeleteEmployee(int id)
    {
        var emp = employees.FirstOrDefault(x => x.Id == id);
        if (emp != null)
        {
            employees.Remove(emp);
            return true;
        }
        return false;
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
 * The purpose of GET method is to retrieve data from the server.
 * The purpose of Http Post method is to create new resource on the server.
 * The purpose of Http Put methid is to update existing resources on the server.
 * The purpose of Http Delete method is to delete existing resources on the server.
 * The purpose of Http Request Headers is to provide additional information about the request or the client itself to the server.
 */

