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
            ILogger log)
        {
            log.LogInformation("Document request started.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var requestDetails = JsonConvert.DeserializeObject<RequestDetails>(requestBody);

            if(requestDetails == null) return (ActionResult)new BadRequestObjectResult("Invalid request body");

            // create id
            // call 3rd party service
            // saved results to db
            // return results

            return (ActionResult)new OkObjectResult($"{requestDetails?.Body}");
        }
    }
}