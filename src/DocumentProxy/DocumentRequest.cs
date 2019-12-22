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
using DocumentProxy.ServiceClient;

namespace DocumentProxy
{
    /// <summary>
    /// Supports initiating a request to the service to process a document.
    /// Note: This function is configured to allow Anonymous access to simplify running the function.
    /// In production security protections would need to be used to protect the service from malicious use.
    /// </summary>
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

                // would probably want some validation of body size
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var requestDetails = JsonConvert.DeserializeObject<RequestDetails>(requestBody);

                if (string.IsNullOrWhiteSpace(requestDetails?.Body)) return new BadRequestObjectResult("Invalid request");

                // create id
                var documentId = Guid.NewGuid();

                // call 3rd party service
                // the documentId is included in the url so that the subsequent callback request can be matched to the original request.
                var callbackUrl = $"/callback/{documentId}";
                var docServiceRequest = new DocServiceRequest
                {
                    Body = requestDetails.Body,
                    Callback = callbackUrl
                };
                // This should be injected with DI
                IDocServiceClient docServiceClient = new DocServiceClient();
                await docServiceClient.RequestAsync(docServiceRequest);

                // saved results to db
                var document = new DocumentDetails
                {
                    Id = documentId,
                    Body = requestDetails.Body,
                    CreatedOn = DateTime.UtcNow
                };
                await documents.AddAsync(document);

                // return results
                return new OkObjectResult($"{documentId}");
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
