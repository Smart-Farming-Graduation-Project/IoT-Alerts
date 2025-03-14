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
            if (data.temperature < -50 || data.temperature > 100 || data.humidity < 0 || data.humidity > 100)
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
            return RecentReadings.Count == 0 ? 0 : RecentReadings.Average(r => r.temperature);
        }
        public double CalculateAverageHumidity()
        {
            return RecentReadings.Count == 0 ? 0 : RecentReadings.Average(r => r.humidity);
        }

        public string GetRiskScore(SensorData data)
        {
            double score = 0;

            if (data.temperature > Thresholds.TempCritical) score += 50;
            else if (data.temperature > Thresholds.TempWarning) score += 20;

            if (data.humidity > Thresholds.HumidityCritical) score += 40;
            else if (data.humidity > Thresholds.HumidityWarning) score += 15;

            if (score > 60) return "🔴 High Risk";
            if (score > 30) return "🟡 Medium Risk";
            return "🟢 Low Risk";
        }

    }
}
