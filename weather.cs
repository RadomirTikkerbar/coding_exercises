using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

class Program
{
    static async Task Main(string[] args)
    {
        // Create HttpClient instance
        using (HttpClient client = new HttpClient())
        {
            // Set the user-agent header as required by the API
            client.DefaultRequestHeaders.Add("User-Agent", "WeatherApp");

            try
            {
                // Define the Yo.no API URL for Tallinn's weather
                string url = "https://api.met.no/weatherapi/locationforecast/2.0/compact?lat=59.437&lon=24.7535";

                // Make the GET request to the API
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // Read the response body as a string
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse the JSON response
                JObject weatherData = JObject.Parse(responseBody);

                // Extract the "timeseries" data from the JSON
                var timeseries = weatherData["properties"]["timeseries"];

                // Loop through the timeseries and print the time and temperature
                foreach (var entry in timeseries)
                {
                    string time = entry["time"].ToString();
                    string temperature = entry["data"]["instant"]["details"]["air_temperature"].ToString();

                    // Print the time and temperature
                    Console.WriteLine($"{time} {temperature}C");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}