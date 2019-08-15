﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Slackbot.Net.Workers.Publishers.Logger;
using Slackbot.Net.Workers.Publishers.Slack;
using Smartbot.Utilities.RecurringActions;
using Smartbot.Utilities.Handlers;

namespace Smartbot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .UseEnvironment(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development")
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddSmartbot(context.Configuration);

                    services.AddSlackbotWorker(o =>
                        {
                            o.Slackbot_SlackApiKey_BotUser = Environment.GetEnvironmentVariable("Slackbot_SlackApiKey_BotUser");
                            o.Slackbot_SlackApiKey_SlackApp = Environment.GetEnvironmentVariable("Slackbot_SlackApiKey_SlackApp");
                        })

                        .AddPublisher<SlackPublisher>()
                        .AddPublisher<LoggerPublisher>()

                        .AddRecurring<HerokuFreeTierKeepAlive>(c => c.Cron = "* */20 * * * *")
                        .AddRecurring<Jorger>(c => { c.Cron = "0 0 9 * * *"; })
                        .AddRecurring<HappyBirthday>(c => c.Cron = "0 0 8 * * *")
                        .AddRecurring<HeartBeater>(c => c.Cron = "0 55 7 * * *")
                        .AddRecurring<Storsdag>(c => c.Cron = "0 0 8 * * THUL")

                        .AddHandler<NesteStorsdagHandler>()
                        .AddHandler<StorsdagerHandler>()
                        .AddHandler<FourSquareHandler>()
                        .AddHandler<OldHandler>()
                        .AddHandler<UrlsSaveHandler>();

                })
                .ConfigureLogging((context, configLogging) =>
                {
                    configLogging
                        .SetMinimumLevel(LogLevel.Trace)
                        .AddConsole(c => c.DisableColors = true)
                        .AddDebug();
                })
                .UseConsoleLifetime()
                .Build();

            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}