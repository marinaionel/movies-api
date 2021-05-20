using System.Threading;
using System.Threading.Tasks;

namespace MoviesApi.Worker
{
    internal interface IScopedProcessingService
    {
        Task DoWork(CancellationToken stoppingToken);
    }
}
