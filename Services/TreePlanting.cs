using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RootedWeb.Services
{
    public class TreePlantingService
    {
        private readonly HttpClient _httpClient;

        public TreePlantingService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> PlantTreeAsync(string username)
        {
            // Create fake payload , what a real API would expect
            var payload = new
            {
                projectId = "12345", // fake project ID
                speciesId = "67890", // fake species ID
                quantity = 1
            };

            var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                // fake sending a POST request to a "tree planting" API endpoint
                var response = await _httpClient.PostAsync("https://jsonplaceholder.typicode.com/posts", content);

                if (response.IsSuccessStatusCode)
                {
                    return $"🌳 Tree planted successfully for {username}!";
                }
                else
                {
                    return $"❌ Tree planting failed for {username}.";
                }
            }
            catch
            {
                return $"❌ Error connecting to tree planting service.";
            }
        }
    }
}
