using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;

namespace BotDispatch.Runner
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
            var result = await dispatchRunner.RunDispatchAsync("-h");

            return new OkObjectResult(result.Output);
        }
    }
}
