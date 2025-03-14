using Azure.Messaging.EventHubs;
using IoT_Alerts.Models;
using IoT_Alerts.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IoT_Alerts.Functions
{
    public static class ProcessSensorDataFunction
    {
        private static readonly SensorDataService _sensorDataService = new SensorDataService();
        private static readonly AlertService _alertService = new AlertService();

        [FunctionName("ProcessSensorData")]
        public static async Task Run(
            [EventHubTrigger("iot-test32", Connection = "EventHubConnectionString")] EventData[] events,
            [SignalR(HubName = "MyHub")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            var tasks = events.Select(eventData => ProcessEvent(eventData, signalRMessages, log));
            await Task.WhenAll(tasks);
        }

        private static async Task ProcessEvent(EventData eventData, IAsyncCollector<SignalRMessage> signalRMessages, ILogger log)
        {
            try
            {
                string eventBody = Encoding.UTF8.GetString(eventData.EventBody.ToArray());
                var sensorData = JsonSerializer.Deserialize<SensorData>(eventBody);

                // Validate sensor data
                if (sensorData == null || !_sensorDataService.IsSensorDataValid(sensorData, log))
                {
                    log.LogWarning("Invalid or null sensor data received.");
                    return;
                }

                // Add to history and calculate averages
                _sensorDataService.AddToHistory(sensorData);
                double avgTemp = _sensorDataService.CalculateAverageTemperature();
                double avgHumidity = _sensorDataService.CalculateAverageHumidity();
                string riskLevel = _sensorDataService.GetRiskScore(sensorData);

                // Analyze sensor data for alerts
                var alerts = _alertService.AnalyzeSensorData(sensorData, log);
                string statusMessage = alerts.Count > 0 ? string.Join("; ", alerts) : $"✅ Normal Data: {sensorData.temperature}°C, {sensorData.humidity}%";

                // Prepare SignalR message
                var response = new
                {
                    CurrentTemperature = sensorData.temperature,
                    CurrentHumidity = sensorData.humidity,
                    AverageTemperature = avgTemp,
                    AverageHumidity = avgHumidity,
                    RiskLevel = riskLevel,
                    StatusMessage = statusMessage
                };

                await signalRMessages.AddAsync(new SignalRMessage
                {
                    Target = "iotMessage",
                    Arguments = new[] { response }
                });

                log.LogInformation("Processed Event: {statusMessage}", statusMessage);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error processing event.");
            }
        }
    }
}