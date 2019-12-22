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
    public static class DocumentRequest
    {
        [FunctionName("request")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "request")]HttpRequest req,
            [CosmosDB(
                databaseName: "DocumentProxy",
                collectionName: "Documents",
                CreateIfNotExists = true,
                ConnectionStringSetting = "CosmosDBConnection")]IAsyncCollector<DocumentDetails> documents,
            ILogger log)
        {
            try
            {
                log.LogInformation("Document request started.");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var requestDetails = JsonConvert.DeserializeObject<RequestDetails>(requestBody);

                if (string.IsNullOrWhiteSpace(requestDetails?.Body)) return (ActionResult)new BadRequestObjectResult("Invalid request");

                // create id
                var documentId = Guid.NewGuid();

                // call 3rd party service


                // saved results to db
                var document = new DocumentDetails
                {
                    Id = documentId,
                    Body = requestDetails.Body
                };
                await documents.AddAsync(document);

                // return results
                return (ActionResult)new OkObjectResult($"{documentId}");
            }
            catch(Exception exception)
            {
                // most likely errors to occur
                //  Error communitcating with 3rd party service
                //  Error communitcating with Cosmos DB
                log.LogError(exception, "Error processing request");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
