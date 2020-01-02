using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Slackbot.Net;
using Slackbot.Net.Configuration;
using Slackbot.Net.Publishers;

namespace Smartbot.Utilities.RecurringActions
{
    public class Jorger : RecurringAction
    {
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly SlackChannels _channels;

        public Jorger(IEnumerable<IPublisher> publishers,
            SlackChannels channels,
            ILogger<Jorger> logger,
            IOptionsSnapshot<CronOptions> options)
            : base(options,logger)
        {
            _publishers = publishers;
            _channels = channels;
        }

        public override async Task Process()
        {
            foreach (var publisher in _publishers)
            {
                var notification = new Notification
                {
                    Msg = $"jorg",
                    Recipient = _channels.JorgChannel
                };
                await publisher.Publish(notification);
            }
        }
    }
}