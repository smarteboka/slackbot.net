using System;
using System.Linq;
using System.Threading.Tasks;
using Slackbot.Net.Endpoints.Abstractions;
using Slackbot.Net.Endpoints.Models.Events;
using Slackbot.Net.SlackClients.Http;

namespace Smartbot.Utilities.Handlers
{
    public class RandomSmartingHandler : IHandleAppMentions
    {
        private readonly Smartinger _smartinger;
        private readonly ISlackClient _client;

        public RandomSmartingHandler(Smartinger smartinger, ISlackClient client)
        {
            _smartinger = smartinger;
            _client = client;
        }

        public (string, string) GetHelpDescription() => ("random, tilfeldig", "Gir deg en tilfeldig smarting");

        public async Task<EventHandledResponse> Handle(EventMetaData data, AppMentionEvent message)
        {
            var index = new Random().Next(_smartinger.Smartingene.Count);
            var randomSmarting = _smartinger.Smartingene[index];
            await _client.ChatGetPermalink(message.Channel, randomSmarting.Name);
            return new EventHandledResponse("OK");
        }

        public bool ShouldHandle(AppMentionEvent message) => Contains(message.Text, "random", "tilfeldig");

        private static bool Contains(string haystack, params string[] needles) => needles.Any(s => haystack.Contains(s, StringComparison.InvariantCultureIgnoreCase));
    }
}