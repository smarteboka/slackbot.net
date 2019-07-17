using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Oldbot.Utilities.FourSquareServices;
using Oldbot.Utilities.SlackAPI.Extensions;
using SlackConnector;
using Smartbot.HostedServices;
using Smartbot.HostedServices.CronServices;
using Smartbot.HostedServices.Strategies;
using Smartbot.Publishers;
using Smartbot.Publishers.Slack;
using Smartbot.Storage;

namespace Smartbot
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCronBots(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SlackOptions>(configuration);
            services.AddSingleton<SlackSender>();
            services.AddSingleton<Smartinger>();
            services.AddSingleton<Timing>();
            services.AddSingleton<SlackChannels>();
            services.AddSingleton<IPublisher, SlackPublisher>();
            services.AddSingleton<IPublisher, ConsolePublisher>();
            services.AddHostedService<JorgingHostedService>();
            services.AddHostedService<BirthdayCheckerHostedService>();
            services.AddHostedService<HeartBeatHostedService>();
            services.AddHostedService<StorsdagsWeekHostedService>();
            return services;
        }
        
        public static IServiceCollection AddSmartbot(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SlackOptions>(configuration);
            services.Configure<SmartStorageOptions>(configuration);
            services.AddSingleton<SlackMessagesStorage>();
            services.AddSingleton<ISlackConnector, SlackConnector.SlackConnector>();
            services.AddSingleton<ISlackClient, SlackTaskClientExtensions>(provider =>
            {
                var config = provider.GetService<IOptions<SlackOptions>>().Value;
                return new SlackTaskClientExtensions(config.SmartBot_SlackApiKey_SlackApp, config.SmartBot_SlackApiKey_BotUser);
            });
            services.AddSingleton<FourSquareService>();
            services.Configure<FourSquareOptions>(configuration);
            services.AddSingleton<StrategySelector>();
            services.AddSingleton<IReplyStrategy, NesteStorsdagStrategy>();
            services.AddSingleton<IReplyStrategy, AllUpcomingStorsdagerStrategy>();
            services.AddSingleton<IReplyStrategy, FoursquareStrategy>();
            services.AddSingleton<IReplyStrategy, OldnessValidatorStrategy>();
            services.AddSingleton<IReplyStrategy, UrlsSaverStrategy>();
            services.AddHostedService<RealTimeBotHostedService>();

            return services;
        }
    }
}