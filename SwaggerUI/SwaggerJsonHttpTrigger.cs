using System.IO;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace SwaggerUI
{
    public static class SwaggerJsonHttpTrigger
    {
        [FunctionName("SwaggerJsonHttpTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "swagger.json")] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var filename = $"{context.FunctionAppDirectory.TrimEnd('/')}/swagger.json";

            var json = default(string);
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                json = await reader.ReadToEndAsync().ConfigureAwait(false);
            }

            var result = new ContentResult()
            {
                StatusCode = (int) HttpStatusCode.OK,
                Content = json,
                ContentType = "application/json",
            };

            return result;
        }
    }
}
