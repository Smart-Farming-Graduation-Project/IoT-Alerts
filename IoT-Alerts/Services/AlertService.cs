using IoT_Alerts.Bases;
using IoT_Alerts.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace IoT_Alerts.Services
{
    public class AlertService
    {
        public List<string> AnalyzeSensorData(SensorData data, ILogger log)
        {
            var alerts = new List<string>();

            if (data.temperature > Thresholds.TempCritical)
            {
                alerts.Add($"🔥 CRITICAL: {data.temperature}°C");
                log.LogError("🔥 CRITICAL: High Temp {temperature}°C", data.temperature);
            }
            else if (data.temperature > Thresholds.TempWarning)
            {
                alerts.Add($"⚠️ Warning: {data.temperature}°C");
                log.LogWarning("⚠️ Warning: High Temp {temperature}°C", data.temperature);
            }
            if (data.humidity > Thresholds.HumidityCritical)
            {
                alerts.Add($"💧 CRITICAL: {data.humidity}%");
                log.LogError("💧 CRITICAL: High Humidity {humidity}%", data.humidity);
            }
            else if (data.humidity > Thresholds.HumidityWarning)
            {
                alerts.Add($"⚠️ Warning: {data.humidity}%");
                log.LogWarning("⚠️ Warning: High Humidity {humidity}%", data.humidity);
            }

            return alerts;
        }
    }
}
