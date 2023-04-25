using GoDaddyDDNS.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoDaddyDDNS
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IDynamicDNSService _dynamicDNSService;
        private readonly int _delay;

        public Worker(ILogger<Worker> logger, IDynamicDNSService dynamicDNSService, IConfiguration configuration)
        {
            _logger=logger;
            _dynamicDNSService=dynamicDNSService;

            var delayInSeconds = configuration.GetValue<int>("ExecutionTimeInSeconds");
            _delay = Convert.ToInt32(TimeSpan.FromSeconds(delayInSeconds).TotalMilliseconds);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Execute(stoppingToken);
                await Task.Delay(_delay, stoppingToken);
            }
        }

        private async Task Execute(CancellationToken cancellationToken)
        {
            try
            {
                await _dynamicDNSService.Execute(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An unexpected error was encountered during execution.");
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting worker...");
            await _dynamicDNSService.Initialize(cancellationToken);
            await base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping worker...");
            return base.StopAsync(cancellationToken);
        }
    }
}
