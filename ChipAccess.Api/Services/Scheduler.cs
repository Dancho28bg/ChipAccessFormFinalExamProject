using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace ChipAccess.Api.Services
{
    public class Scheduler : BackgroundService
    {
        private readonly ILogger<Scheduler> _logger;
        private readonly IServiceProvider _serviceProvider;

        private readonly TimeSpan _interval = TimeSpan.FromHours(6);

        public Scheduler(ILogger<Scheduler> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Scheduler started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using(var scope = _serviceProvider.CreateScope())
                    {
                        var accessService = scope.ServiceProvider.GetRequiredService<IAccessService>();

                        _logger.LogInformation("Scheduler: running expiration checks...");

                        var expired = await accessService.UpdateExpiredStatusesAsync();
                        var soon = await accessService.UpdateExpiringSoonStatusesAsync();

                        _logger.LogInformation("Expired updated: {count}", expired);
                        _logger.LogInformation("Expiring soon updated: {count}", soon);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Scheduler error occurred.");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("Scheduler stopped.");
        }
    }
}