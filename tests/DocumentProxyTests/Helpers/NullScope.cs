using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentProxyTests.Helpers
{
    /// <summary>
    /// Src from: https://docs.microsoft.com/en-us/azure/azure-functions/functions-test-a-function
    /// </summary>
    public class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new NullScope();

        private NullScope() { }

        public void Dispose() { }
    }
}
