using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundServices;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace BackgroundProcessPoc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackgroundJobsController : ControllerBase
    {
        private readonly IDataService _dataService;

        public BackgroundJobsController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public void Post([FromBody] TestRequest request)
        {
            BackgroundJob.Enqueue(() => Console.WriteLine(request.Data));

            BackgroundJob.Enqueue<TestService>(x => x.DoTask());

            RecurringJob.AddOrUpdate(request.JobId, () => new TestService().DoTask(), Cron.MinuteInterval(2));
            RecurringJob.AddOrUpdate<IDataService>(request.JobId + "_1", x => x.AddData() , Cron.Minutely);
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
