using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

class Program
{
    static async Task Main(string[] args)
    {
        
        using (HttpClient client = new HttpClient())
        {
            
            client.DefaultRequestHeaders.Add("User-Agent", "WeatherApp");

            try
            {
                
                string url = "https://api.met.no/weatherapi/locationforecast/2.0/compact?lat=59.437&lon=24.7535";

                
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                
                string responseBody = await response.Content.ReadAsStringAsync();

                
                JObject weatherData = JObject.Parse(responseBody);

                
                var timeseries = weatherData["properties"]["timeseries"];

                
                foreach (var entry in timeseries)
                {
                    string time = entry["time"].ToString();
                    string temperature = entry["data"]["instant"]["details"]["air_temperature"].ToString();

                    
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
