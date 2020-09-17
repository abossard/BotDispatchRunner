using System.IO;
using System.Threading.Tasks;
using BotDispatch.NPM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BotDispatch.Function
{
    public static class HttpTriggerDispatch
    {
        [FunctionName("HttpTriggerDispatch")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var workingDirectory = Path.Join(Path.GetTempPath(), "dispatch");
            log.LogInformation(workingDirectory);
            var dispatchRunner = new DispatchRunner(workingDirectory);
            var command = @"init -n mymodel --luisAuthoringKey 235e10f069154d48aa3729da6a13d36a --luisAuthoringRegion westeurope --dataFolder c:\\temp\\dispatch";
            var result = await dispatchRunner.RunDispatchAsync(command);
            return new OkObjectResult(result.Output);
        }
    }
}
