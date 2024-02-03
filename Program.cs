using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

class WeatherApp
{
    private static readonly HttpClient httpClient = new HttpClient();
    private static string apiKey;
    private static IConfigurationRoot configuration;

    static WeatherApp()
    {
        configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        httpClient.BaseAddress = new Uri(configuration["WeatherApi:BaseAddress"]);
        apiKey = configuration["WeatherApi:ApiKey"];
    }

    public static async Task Main(string[] args)
    {
        Console.Write("Enter a zipcode: ");
        string zipcode = Console.ReadLine();

        try
        {
            var weatherData = await GetWeatherDataAsync(zipcode);
            DisplayWeatherInfo(weatherData);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private static async Task<WeatherData> GetWeatherDataAsync(string zipcode)
    {
        HttpResponseMessage response = await httpClient.GetAsync($"?access_key={apiKey}&query={zipcode}");
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<WeatherData>(responseBody);
    }

    private static void DisplayWeatherInfo(WeatherData data)
    {
        bool isRaining = false;
        if(data.Current.Weather_Descriptions.Contains("Raining")){
            isRaining = true;
        }
        Console.WriteLine("Should I go outside?");
        Console.WriteLine(isRaining ? "No" : "Yes");

        Console.WriteLine("Should I wear sunscreen?");
        Console.WriteLine(data.Current.UvIndex > 3 ? "Yes" : "No");

        Console.WriteLine("Can I fly my kite?");
        Console.WriteLine(!isRaining && data.Current.WindSpeed > 15 ? "Yes" : "No");
    }
}

public class WeatherData
{
    public CurrentWeather Current { get; set; }
}

public class CurrentWeather
{
    public int Temperature { get; set; }
    public int WindSpeed { get; set; }
    public int UvIndex { get; set; }
    public string[] Weather_Descriptions { get; set; }
}

