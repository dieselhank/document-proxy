using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentProxy.Models
{
    public class DocumentStatusResponse
    {
        public string Status { get; set; }
        public string Detail { get; set; }
        public string Body { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
