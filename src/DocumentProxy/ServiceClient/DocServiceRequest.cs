using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentProxy.ServiceClient
{
    public class DocServiceRequest
    {
        /// <summary>
        /// body of the request to be forwarded to the 3rd party service.
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// Callback url for the 3rd party service to use.
        /// </summary>
        public string Callback { get; set; }
    }
}
