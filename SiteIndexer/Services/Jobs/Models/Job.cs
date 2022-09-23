using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteIndexer.Services.Jobs.Models
{
    public class Job
    {
        public string Handle { get; set; }
        public Action<MessageList> JobFunction { get; set; }
        public MessageList MessageList { get; set; }

        public Job(string jobHandle, Action<MessageList> jobFunction)
        {
            Handle = jobHandle;
            JobFunction = jobFunction;
            MessageList = new MessageList();
        }

        public void Run(IJobService jobService)
        {
            JobFunction(MessageList);

            jobService.FinishJob(this);
        }
    }
}