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
    /// Supports status update callbacks using a PUT by the 3rd party service.
    /// This function is configured to allow Anonymous access to simplify running the function.
    /// In production security protections would need to be used to protect the service from malicious use.
    /// </summary>
    public static class DocumentPutCallback
    {
        private static string[] _validStatus = new string[] { "PROCESSED", "COMPLETED", "ERROR" };

        [FunctionName("callbackPut")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "callback/{id}")] HttpRequest req, string id,
            [CosmosDB(
                databaseName: "DocumentProxy",
                collectionName: "Documents",
                Id = "{id}",
                CreateIfNotExists = true,
                ConnectionStringSetting = "CosmosDBConnection")] DocumentDetails document,
            ILogger log)
        {
            log.LogInformation($"Status update callback request for {id}.");

            // would probably want some validation of body size
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var requestDetails = JsonConvert.DeserializeObject<StatusUpdateRequest>(requestBody);

            if (string.IsNullOrWhiteSpace(requestDetails?.Status)) return new BadRequestObjectResult("Invalid request");
            if (!_validStatus.Contains(requestDetails.Status)) return new BadRequestObjectResult($"Invalid status {requestDetails.Status}");

            if (document == null) return new BadRequestObjectResult("Invalid request");

            document.Status.Add(new DocumentStatus {
                Status = requestDetails.Status,
                Detail = requestDetails.Detail,
                CreatedOn = DateTime.UtcNow });

            // possible error conditions
            //  duplicate calls
            //  document for id not found
            //  cosmos db connection timeout/errors

            return new NoContentResult();
        }
    }
}
