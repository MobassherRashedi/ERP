using DevSkill.Inventory.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Infrastructure.UnitOfWorks
{
    public abstract class UnitOfWork : IUnitOfWork
    {
        protected readonly DbContext _dbContext;
        private readonly ILogger<UnitOfWork> _logger;

        public UnitOfWork(DbContext dbContext, ILogger<UnitOfWork> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Dispose()
        {
            try
            {
                _dbContext?.Dispose();
                Console.WriteLine("DbContext is being disposed");
                _logger.LogInformation("DbContext disposed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while disposing the DbContext.");
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await _dbContext.DisposeAsync();
                _logger.LogInformation("DbContext disposed asynchronously successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while disposing the DbContext asynchronously.");
                throw;
            }
        }

        public void Save()
        {
            try
            {
                _dbContext?.SaveChanges();
                _logger.LogInformation("Changes saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving changes to the DbContext.");
                throw;  // Re-throw the exception to handle it further up the call stack if necessary
            }
        }

        public async Task SaveAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Changes saved asynchronously successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving changes asynchronously to the DbContext.");
                throw;  // Re-throw the exception to handle it further up the call stack if necessary
            }
        }
    }
}
