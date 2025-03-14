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
        private static readonly List<SensorData> RecentReadings = new List<SensorData>();
        public bool IsSensorDataValid(SensorData data, ILogger log)
        {
            if (data.Temperature < -50 || data.Temperature > 100 || data.Humidity < 0 || data.Humidity > 100)
            {
                log.LogWarning("Invalid sensor data: {data}", data);
                return false;
            }
            return true;
        }

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

            if (score > 60) return "🔴 High Risk";
            if (score > 30) return "🟡 Medium Risk";
            return "🟢 Low Risk";
        }

    }
}
