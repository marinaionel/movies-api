using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MoviesApi.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesApi.Worker
{
    public class DataFillingService : BackgroundService
    {
        public DataFillingService(IServiceProvider services)
        {
            Services = services;
        }

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Default.Info("Data Filling Service running.");

            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            Log.Default.Info("Consume Scoped Service Hosted Service is working.");

            using IServiceScope scope = Services.CreateScope();
            IScopedProcessingService scopedProcessingService = scope.ServiceProvider.GetRequiredService<IScopedProcessingService>();

            await scopedProcessingService.DoWork(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            Log.Default.Info("Data Filling Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
