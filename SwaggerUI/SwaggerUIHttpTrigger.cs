using System;
using System.Globalization;
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

            var html = default(string);
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                html = await reader.ReadToEndAsync().ConfigureAwait(false);
            }

            var openapi = default(string);
            var openapiDocUri = Environment.GetEnvironmentVariable("OpenApi__Document");

            // OpenAPI document comes from external source
            if (openapiDocUri.StartsWith("http", ignoreCase: true, CultureInfo.InvariantCulture))
            {
                openapi = openapiDocUri;
            }
            // OpenAPI document hosted in the app
            else
            {
                openapi = $"{req.Scheme}://{req.Host.Value.TrimEnd('/')}/{Environment.GetEnvironmentVariable("OpenApi__Document").TrimStart('/')}";
            }

            html = html.Replace("[[OPENAPI_DOCUMENT_LOCATION]]", openapi);

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
