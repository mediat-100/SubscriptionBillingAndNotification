using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SubscriptionBillingAndNotificationCore.Contracts.IService;

namespace SubscriptionBillingAndNotificationCore.Infrastructure.Service.Background
{
    public class SubscriptionReminderService : IHostedService
    {
        private readonly ILogger<SubscriptionReminderService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly PeriodicTimer _timer = new PeriodicTimer(TimeSpan.FromHours(8));
        public SubscriptionReminderService(ILogger<SubscriptionReminderService> logger, 
            IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Subscription Reminder Service is starting");

            var executingTask = ExecuteAsync(cancellationToken);

            return Task.CompletedTask;
        }


        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (await _timer.WaitForNextTickAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var userSubscriptionService = scope.ServiceProvider
                            .GetRequiredService<IUserSubscriptionService>();
                        _logger.LogInformation($"Starting to process advance subscription reminders at {DateTime.UtcNow}");
                        await userSubscriptionService.ProcessAdvanceReminders(cancellationToken);
                        _logger.LogInformation($"Starting to process advance subscription reminders at {DateTime.UtcNow}");
                        await userSubscriptionService.ProcessExpiryDayReminders(cancellationToken);
                    }
                       
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Cancellation received!");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing subscription reminders at {DateTime.UtcNow}");
                }
            }
         
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Subscription Reminder Service stopped");
            return Task.CompletedTask;
        }

    }
}
