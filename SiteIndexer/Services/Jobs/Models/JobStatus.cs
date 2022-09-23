using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteIndexer.Services.Jobs.Models
{
    public class JobStatus
    {
        public bool IsFinished { get; set; }
        public List<string> Messages { get; set; }
        public string JobHandle { get; set; }
        public DateTime LastReceived { get; set; }

        public JobStatus()
        {
            Messages = new List<string>();
        }
    }
}