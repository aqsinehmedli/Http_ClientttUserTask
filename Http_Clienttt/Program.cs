using Http_Clienttt;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Client
{
    private static readonly HttpClient httpClient = new HttpClient();

    public static async Task AddUser()
    {
        var user = new UserTask
        {
            FirstName = "Aqsin",
            LastName = "Ehmedli",
            EmailAdress = "aqsinehmedli3@gmail.com"
        };

        var json = JsonSerializer.Serialize(user);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await httpClient.PostAsync("http://localhost:5000/", content);
            Console.WriteLine("Server response: " + await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while adding user: " + ex.Message);
        }
    }

    public static async Task DeleteUser(int userId)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"http://localhost:5000/{userId}");
            Console.WriteLine("Server response: " + await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while deleting user: " + ex.Message);
        }
    }

    public static async Task Main()
    {
        try
        {
            var response = await httpClient.GetStringAsync("http://localhost:5000/");
            Console.WriteLine("Server response: " + response);

            await AddUser();

            await DeleteUser(1); 
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
