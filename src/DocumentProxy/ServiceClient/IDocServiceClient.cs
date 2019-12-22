using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentProxy.ServiceClient
{
    public interface IDocServiceClient
    {
        Task RequestAsync(DocServiceRequest request);
    }
}
