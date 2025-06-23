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

            if (data.Temperature > Thresholds.TempCritical)
            {
                alerts.Add($"🔥 CRITICAL: {data.Temperature}°C");
                log.LogError("🔥 CRITICAL: High Temp {temperature}°C", data.Temperature);
            }
            else if (data.Temperature > Thresholds.TempWarning)
            {
                alerts.Add($"⚠️ Warning: {data.Temperature}°C");
                log.LogWarning("⚠️ Warning: High Temp {temperature}°C", data.Temperature);
            }
            if (data.Humidity > Thresholds.HumidityCritical)
            {
                alerts.Add($"💧 CRITICAL: {data.Humidity}%");
                log.LogError("💧 CRITICAL: High Humidity {humidity}%", data.Humidity);
            }
            else if (data.Humidity > Thresholds.HumidityWarning)
            {
                alerts.Add($"⚠️ Warning: {data.Humidity}%");
                log.LogWarning("⚠️ Warning: High Humidity {humidity}%", data.Humidity);
            }
            if (data.Flame > Thresholds.FlameDetected)
            {
                alerts.Add("🚨 FLAME DETECTED!");
                log.LogError("🚨 FLAME DETECTED! Value: {flame}", data.Flame);
            }

            return alerts;
        }
    }
}
