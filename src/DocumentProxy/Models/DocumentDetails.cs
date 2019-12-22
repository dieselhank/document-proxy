using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentProxy.Models
{
    /// <summary>
    /// DTO used to store data in Cosmos DB.
    /// Partition Key is being ignored for this implementation. But for production code partitioning is needed.
    /// </summary>
    public class DocumentDetails
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
    }
}
