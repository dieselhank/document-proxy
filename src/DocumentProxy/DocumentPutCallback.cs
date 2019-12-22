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
    public static class DocumentPutCallback
    {
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
            log.LogInformation("C# HTTP trigger function processed a request.");

            return new NoContentResult();
        }
    }
}
