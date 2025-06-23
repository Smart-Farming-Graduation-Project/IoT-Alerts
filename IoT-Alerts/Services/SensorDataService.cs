using IoT_Alerts.Bases;
using IoT_Alerts.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace IoT_Alerts.Services
{
    public class SensorDataService
    {
        private const int MaxReadings = 10; // Keep last 10 readings
        private readonly List<SensorData> RecentReadings = new List<SensorData>();
        public bool IsSensorDataValid(SensorData data, ILogger log)
        {
            if (data.Temperature < -50 || data.Temperature > 100 || data.Humidity < 0 || data.Humidity > 100)
            {
                log.LogWarning("Invalid sensor data: {data}", data);
                return false;
            }
            if (data.Flame < 0 || data.Flame > 1023)
            {
                log.LogWarning("Invalid flame sensor value: {flame}", data.Flame);
                return false;
            }
            return true;
        }
        //Future implementation

        //private static async Task<string> GetWeatherAsync()
        //{
        //    string apiKey = "YOUR_WEATHER_API_KEY"; // Replace with your actual API key
        //    string city = "YourCity";
        //    string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";

        //    var response = await httpClient.GetStringAsync(url);
        //    return response;
        //}

        //private static async Task<double> PredictFutureTemperature()
        //{
        //    string modelUrl = "YOUR_AZURE_ML_ENDPOINT"; // Replace with your Azure ML endpoint
        //    var response = await httpClient.GetStringAsync(modelUrl);
        //    return double.Parse(response);
        //}
        //private static async Task SendSmsAlert(string message)
        //{
        //    string twilioUrl = "https://api.twilio.com/2010-04-01/Accounts/YOUR_ACCOUNT_SID/Messages.json";
        //    var content = new FormUrlEncodedContent(new[]
        //    {
        //        new KeyValuePair<string, string>("To", "+1234567890"),
        //        new KeyValuePair<string, string>("From", "YourTwilioNumber"),
        //        new KeyValuePair<string, string>("Body", message)
        //    });

        //    HttpResponseMessage response = await httpClient.PostAsync(twilioUrl, content);
        //}
        public void AddToHistory(SensorData data)
        {
            if (RecentReadings.Count >= MaxReadings)
            {
                RecentReadings.RemoveAt(0); // Remove oldest reading
            }
            RecentReadings.Add(data);
        }

        public double CalculateAverageTemperature()
        {
            return RecentReadings.Count == 0 ? 0 : RecentReadings.Average(r => r.Temperature);
        }
        public double CalculateAverageHumidity()
        {
            return RecentReadings.Count == 0 ? 0 : RecentReadings.Average(r => r.Humidity);
        }

        public string GetRiskScore(SensorData data)
        {
            double score = 0;

            if (data.Temperature > Thresholds.TempCritical) score += 50;
            else if (data.Temperature > Thresholds.TempWarning) score += 20;

            if (data.Humidity > Thresholds.HumidityCritical) score += 40;
            else if (data.Humidity > Thresholds.HumidityWarning) score += 15;
            if (data.Flame > Thresholds.FlameDetected) score += 100;
            if (score > 60) return "🔴 High Risk";
            if (score > 30) return "🟡 Medium Risk";
            return "🟢 Low Risk";
        }

    }
}
