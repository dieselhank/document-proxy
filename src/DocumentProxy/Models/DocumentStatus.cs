using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentProxy.Models
{
    public class DocumentStatus
    {
        public string Status { get; set; }
        public string Detail { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
