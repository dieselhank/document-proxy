using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DocumentProxyTests.Helpers
{
    /// <summary>
    /// Src from: https://docs.microsoft.com/en-us/azure/azure-functions/functions-test-a-function
    /// </summary>
    public class TestFactory
    {
        public static DefaultHttpRequest CreateHttpRequest(string body)
        {
            var byteArray = Encoding.UTF8.GetBytes(body);
            var stream = new MemoryStream(byteArray);

            var request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = stream
            };
            return request;
        }

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;

            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }

            return logger;
        }
    }
}
