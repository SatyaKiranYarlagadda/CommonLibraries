using System;
using System.Threading.Tasks;
using BackgroundServices.Models;
using Microsoft.Extensions.Logging;

namespace BackgroundServices
{
    public class DataService : IDataService
    {
        private readonly BackgroundTasksContext _dbContext;
        private readonly ILogger<DataService> _logger;

        public DataService(BackgroundTasksContext dbContext, ILogger<DataService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task AddData()
        {
            var dateTime = DateTime.Now;

            _logger.LogInformation($"Adding DateTime:{dateTime} to the database.");
            await _dbContext.TestData.AddAsync(new TestData
            {
                Value = dateTime
            });

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Saved changes [DateTime:{dateTime}] to the database.");
        }
    }
}
