using Http_Clienttt;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    private static HttpListener _listener;

    public static async Task Main()
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add("http://localhost:5000/");
        _listener.Start();
        Console.WriteLine("HTTP server started on port 5000");

        while (true)
        {
            var context = await _listener.GetContextAsync();
            _ = HandleRequest(context);
        }
    }

    private static async Task HandleRequest(HttpListenerContext context)
    {
        HttpListenerRequest request = context.Request;

        try
        {
            if (request.HttpMethod == "GET")
            {
                string data = await FetchDataFromDatabaseAsync();
                byte[] responseBuffer = Encoding.UTF8.GetBytes(data);
                context.Response.ContentLength64 = responseBuffer.Length;
                await context.Response.OutputStream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
            }
            else if (request.HttpMethod == "POST")
            {
                string responseMessage = await AddUserAsync(request);
                byte[] responseBuffer = Encoding.UTF8.GetBytes(responseMessage);
                context.Response.ContentLength64 = responseBuffer.Length;
                await context.Response.OutputStream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
            }
            else if (request.HttpMethod == "DELETE")
            {
                string id = request.Url.AbsolutePath.Split('/').Last(); 
                string responseMessage = await DeleteUserAsync(id);
                byte[] responseBuffer = Encoding.UTF8.GetBytes(responseMessage);
                context.Response.ContentLength64 = responseBuffer.Length;
                await context.Response.OutputStream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error handling request: " + ex.Message);
            context.Response.StatusCode = 500; 
            var errorResponse = Encoding.UTF8.GetBytes("Internal Server Error");
            context.Response.ContentLength64 = errorResponse.Length;
            await context.Response.OutputStream.WriteAsync(errorResponse, 0, errorResponse.Length);
        }
        finally
        {
            context.Response.OutputStream.Close();
        }
    }

    private static async Task<string> DeleteUserAsync(string id)
    {
        using (var context = new UserTaskContext())
        {
            if (int.TryParse(id, out int userId))
            {
                var user = await context.UserTasks.FindAsync(userId);
                if (user != null)
                {
                    context.UserTasks.Remove(user);
                    await context.SaveChangesAsync();
                    return $"User with ID {userId} deleted successfully.";
                }
                else
                {
                    return $"User with ID {userId} not found.";
                }
            }
            else
            {
                return "Invalid ID format.";
            }
        }
    }

    private static async Task<string> FetchDataFromDatabaseAsync()
    {
        StringBuilder dataBuilder = new StringBuilder();

        using (var context = new UserTaskContext())
        {
            var tasks = await context.UserTasks.ToListAsync();
            foreach (var task in tasks)
            {
                dataBuilder.AppendLine($"{task.Id}: {task.FirstName}: {task.LastName}: {task.EmailAdress}");
            }
        }

        return dataBuilder.ToString();
    }

    private static async Task<string> AddUserAsync(HttpListenerRequest request)
    {
        using (var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding))
        {
            string requestBody = await reader.ReadToEndAsync();
            var userData = JsonSerializer.Deserialize<UserTask>(requestBody);

            using (var context = new UserTaskContext())
            {
                context.UserTasks.Add(userData);
                await context.SaveChangesAsync();
            }

            return "User added successfully.";
        }
    }
}
