using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteIndexer.Services.Jobs.Models
{
    public class MessageList
    {
        public Dictionary<string, DateTime> Messages { get; set; }

        public MessageList()
        {
            Messages = new Dictionary<string, DateTime>();
        }

        public void Add(string message)
        {
            Messages[message] = DateTime.Now;
        }
    }
}