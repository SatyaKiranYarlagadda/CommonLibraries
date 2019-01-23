using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundServices;
using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BackgroundProcessPoc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackgroundJobsController : ControllerBase
    {
        private readonly IDataService _dataService;
        private readonly BackgroundProcessConfig _backgroundProcessConfig;

        public BackgroundJobsController(IDataService dataService, IOptions<BackgroundProcessConfig> backgroundProcessConfig)
        {
            _dataService = dataService;
            _backgroundProcessConfig = backgroundProcessConfig.Value;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                var recurringJobs = connection.GetRecurringJobs();
                return Ok(recurringJobs.Select(x => x.Id));
            }
        }

        [HttpGet("{jobId}")]
        public ActionResult<string> Get(string jobId)
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                var job = connection.GetRecurringJobs().FirstOrDefault(x => x.Id == jobId);
                return Ok(JsonConvert.SerializeObject(new
                {
                    JobId = job.Id,
                    Queue = job.Queue,
                    Schedule = job.Cron,
                    CreatedAt = job.CreatedAt,
                    LastExecuted = job.LastExecution,
                    LastJobId = job.LastJobId,
                    LastJobState = job.LastJobState,
                    NextExecution = job.NextExecution
                }));
            }
        }

        [HttpPost]
        public void Post([FromBody] TestRequest request)
        {
            BackgroundJob.Enqueue(() => Console.WriteLine(request.Data));

            BackgroundJob.Enqueue<TestService>(x => x.DoTask());

            RecurringJob.AddOrUpdate(request.JobId, () => new TestService().DoTask(), Cron.MinuteInterval(2), queue:_backgroundProcessConfig.Queue);
            RecurringJob.AddOrUpdate<IDataService>(request.JobId + "_1", x => x.AddData() , Cron.Minutely, queue: _backgroundProcessConfig.Queue);
        }

        [HttpPost("trigger/{jobId}")]
        public void Post([FromRoute] string jobId)
        {
            RecurringJob.Trigger(jobId);
        }

        [HttpDelete("{jobId}")]
        public void Delete(string jobId)
        {
            RecurringJob.RemoveIfExists(jobId);
        }
    }
}
