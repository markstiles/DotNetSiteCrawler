using SiteIndexer.Services.Jobs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace SiteIndexer.Services.Jobs
{
    public interface IJobService
    {
        string StartJob(Guid crawlerId, Action<Guid, MessageList> jobFunction);
        void FinishJob(Job job);
        JobStatus GetJobStatus(string jobHandle, DateTime lastDateReceived);
    }

    public class JobService : IJobService
    {
        private readonly Dictionary<string, Job> RunningJobs = new Dictionary<string, Job>();
        private readonly Dictionary<string, Job> FinishedJobs = new Dictionary<string, Job>();
        private readonly object lockObject = new object();

        public JobService()
        {

        }

        public string StartJob(Guid crawlerId, Action<Guid, MessageList> jobFunction)
        {
            lock (lockObject)
            {
                var jobHandle = Guid.NewGuid().ToString();
                var j = new Job(jobHandle, crawlerId, jobFunction);
                var jobThread = new Thread(() => j.Run(this));
                jobThread.Start();
                RunningJobs.Add(jobHandle, j);

                return jobHandle;
            }
        }

        public JobStatus GetJobStatus(string jobHandle, DateTime lastDateReceived)
        {
            lock (lockObject)
            {
                var updatedLastDate = DateTime.Now;
                var isRunning = RunningJobs.ContainsKey(jobHandle);
                var isFinished = FinishedJobs.ContainsKey(jobHandle);
                var jobList = isRunning ? RunningJobs : FinishedJobs;

                var messages = jobList[jobHandle]?
                    .MessageList.Messages
                    .Where(a => a.Value > lastDateReceived)
                    .Select(b => b.Key)
                    .ToList() 
                    ?? new List<string>();

                if (!isRunning && isFinished)
                    messages.Add("Job finished");
                else if (!isRunning && !isFinished) // not found and probably old or removed
                    messages.Add("Job not found");

                return new JobStatus
                {
                    Messages = messages,
                    IsFinished = !isRunning,
                    JobHandle = jobHandle,
                    LastReceived = updatedLastDate
                };
            }
        }

        public void FinishJob(Job job) 
        {
            lock (lockObject)
            {
                RunningJobs.Remove(job.Handle);
                FinishedJobs.Add(job.Handle, job);
            }
        }
    }
}