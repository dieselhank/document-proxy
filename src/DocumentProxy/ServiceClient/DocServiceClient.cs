using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentProxy.ServiceClient
{
    /// <summary>
    /// This class would support calling the 3rd party service at http://example.com/request.
    /// HttpClient can be used to call to the service with the payload contained in the request object.
    /// Best practices for using HttpClient would need to be followed.
    /// 
    /// </summary>
    public class DocServiceClient : IDocServiceClient
    {
        public Task RequestAsync(DocServiceRequest request)
        {
            // call 3rd party service with HttpClient
            // errors could be caught and logged here
            // but errors should be propagated up to the Function to be caught so proper responses can be generated.
            // some retry logic could be implemented but Azure Functions do have a timeout.
            return Task.CompletedTask;
        }
    }
}
