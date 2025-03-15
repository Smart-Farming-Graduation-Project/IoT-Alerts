using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;

namespace IoT_Alerts.Functions
{
    public static class NegotiateFunction
    {
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo GetSignalRInfo(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req,
            [SignalRConnectionInfo(HubName = "MyHub")] SignalRConnectionInfo connectionInfo, ILogger log)
        {
            // Log the URL and access token
            log.LogInformation($"SignalR URL: {connectionInfo.Url}");
            log.LogInformation($"SignalR Access Token: {connectionInfo.AccessToken}");
            return connectionInfo;
        }
    }
}