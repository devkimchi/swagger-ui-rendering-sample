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
    public static class SwaggerUIHttpTrigger
    {
        [FunctionName("SwaggerUIHttpTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "swagger/ui")] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var filename = $"{context.FunctionAppDirectory.TrimEnd('/')}/index.html";

            log.LogInformation(filename);

            var html = default(string);
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                html = await reader.ReadToEndAsync().ConfigureAwait(false);
            }

            log.LogInformation(html);

            var result = new ContentResult()
            {
                StatusCode = (int) HttpStatusCode.OK,
                Content = html,
                ContentType = "text/html",
            };

            return result;
        }
    }
}
