using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace console
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient() { BaseAddress = new Uri("https://services.odata.org/TripPinRESTierService/(S(f3u1x5y4m11keyabeqbctnqd))/") };

        static async Task Main(string[] args)
        {
            var users = JsonSerializer.Deserialize<User[]>(await File.ReadAllTextAsync("users.json"));

            foreach (var user in users)
            {
                if (!await DoesUserExist(user))
                {
                    Console.WriteLine($"Adding user {user.UserName}.");
                    await AddUser(user);
                }
            }
        }

        public static async Task<bool> DoesUserExist(User user)
        {
            return (await client.GetAsync($"People('{user.UserName}')")).IsSuccessStatusCode;
        }

        public static async Task<bool> AddUser(User user)
        {
            var newUser = new
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Emails = new[] {
                    user.Email
                },
                AddressInfo = new[] {
                    new {
                        Address = user.Address,
                        City = new  {
                            Name = user.CityName,
                            CountryRegion = user.Country,
                            Region = "unknown"
                        }
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(newUser), Encoding.UTF8, "application/json");
            return (await client.PostAsync("People", content)).IsSuccessStatusCode;
        }
    }
}
