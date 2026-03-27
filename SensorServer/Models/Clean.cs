using Version2.Models.Version2.Models;

namespace Version2.Models
{
    public class StateCleanupService : BackgroundService
    {
        private readonly ClientStateManager _clientStateManager;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(5); // Cleanup every 5 minutes

        public StateCleanupService(ClientStateManager clientStateManager)
        {
            _clientStateManager = clientStateManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _clientStateManager.CleanupExpiredStates();
                await Task.Delay(_cleanupInterval, stoppingToken);
            }
        }
    }

}
