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
using System.Linq;

namespace DocumentProxy
{
    /// <summary>
    /// Supports getting the current status of a previous document request.
    /// This function is configured to allow Anonymous access to simplify running the function.
    /// In production security protections would need to be used to protect the service from malicious use.
    /// </summary>
    public static class DocumentStatusRequest
    {
        [FunctionName("status")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "status/{id}")] HttpRequest req, string id,
            [CosmosDB(
                databaseName: "DocumentProxy",
                collectionName: "Documents",
                Id = "{id}",
                CreateIfNotExists = true,
                ConnectionStringSetting = "CosmosDBConnection")] DocumentDetails document,
            ILogger log)
        {
            log.LogInformation($"Get Status for {id}.");

            if (document == null) return new BadRequestObjectResult("Invalid request");

            var latestUpdate = document.Status.LastOrDefault();
            var response = new DocumentStatusResponse
            {
                Body = document.Body,
                CreatedOn = document.CreatedOn,
                Status = latestUpdate?.Status,
                Detail = latestUpdate?.Detail,
                UpdatedOn = latestUpdate == null ? document.CreatedOn : latestUpdate.CreatedOn
            };

            return new OkObjectResult(response);
        }
    }
}
