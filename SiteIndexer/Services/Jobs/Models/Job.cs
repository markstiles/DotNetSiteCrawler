using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteIndexer.Services.Jobs.Models
{
    public class Job
    {
        public string Handle { get; set; }
        public Action<Guid, MessageList> JobFunction { get; set; }
        public MessageList MessageList { get; set; }
        public Guid CrawlerId { get; set; } 

        public Job(string jobHandle, Guid crawlerId, Action<Guid, MessageList> jobFunction)
        {
            Handle = jobHandle;
            JobFunction = jobFunction;
            MessageList = new MessageList();
            CrawlerId = crawlerId;
        }

        public void Run(IJobService jobService)
        {
            JobFunction(CrawlerId, MessageList);

            jobService.FinishJob(this);
        }
    }
}