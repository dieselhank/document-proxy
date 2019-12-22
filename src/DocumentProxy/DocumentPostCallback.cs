using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DocumentProxy.Models;

namespace DocumentProxy
{
    /// <summary>
    /// Supports the first callback using a POST by the 3rd party service.
    /// This function is configured to allow Anonymous access to simplify running the function.
    /// In production security protections would need to be used to protect the service from malicious use.
    /// </summary>
    public static class DocumentPostCallback
    {
        [FunctionName("callbackPost")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "callback/{id}")] HttpRequest req, string id,
            [CosmosDB(
                databaseName: "DocumentProxy",
                collectionName: "Documents",
                Id = "{id}",
                CreateIfNotExists = true,
                ConnectionStringSetting = "CosmosDBConnection")] DocumentDetails document,
            ILogger log)
        {
            log.LogInformation($"Initial callback request for {id}.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            if(requestBody != "STARTED") return new BadRequestObjectResult("Invalid request");
            if(document == null) return new BadRequestObjectResult("Invalid request");

            document.Status.Add(new DocumentStatus {
                Status = "STARTED",
                Detail = "",
                CreatedOn = DateTime.UtcNow });

            // possible error conditions
            //  duplicate calls
            //  document for id not found
            //  cosmos db connection timeout/errors

            return new NoContentResult();
        }
    }
}
