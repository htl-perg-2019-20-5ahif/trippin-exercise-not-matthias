class User { public string UserName, FirstName, LastName, Email, Address, CityName, Country; }
class Program {
    private static readonly System.Net.Http.HttpClient client = new System.Net.Http.HttpClient() { BaseAddress = new System.Uri("https://services.odata.org/TripPinRESTierService/(S(f3u1x5y4m11keyabeqbctnqd))/") };
    static async System.Threading.Tasks.Task Main(string[] args) {
        foreach (var user in System.Text.Json.JsonSerializer.Deserialize<User[]>(await System.IO.File.ReadAllTextAsync("users.json"))) // Loop through every user read from the file
            if (!(await client.GetAsync($"People('{user.UserName}')")).IsSuccessStatusCode) // Check if user exists
                await client.PostAsync("People", new System.Net.Http.StringContent(System.Text.Json.JsonSerializer.Serialize(new { user.UserName, user.FirstName, user.LastName, Emails = new[] { user.Email }, AddressInfo = new[] { new { user.Address, City = new { Name = user.CityName, CountryRegion = user.Country, Region = "unknown" } } } }), System.Text.Encoding.UTF8, "application/json"));
    }
}