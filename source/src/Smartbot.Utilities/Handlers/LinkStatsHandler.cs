using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slackbot.Net.Abstractions.Handlers;
using Slackbot.Net.Abstractions.Handlers.Models.Rtm.MessageReceived;
using Slackbot.Net.Abstractions.Publishers;
using Slackbot.Net.SlackClients.Http;
using Smartbot.Utilities.Storage.SlackUrls;

namespace Smartbot.Utilities.Handlers
{
    public class LinkStatsHandler : IHandleMessages
    {
        private readonly IEnumerable<IPublisher> _publishers;
        private readonly ISlackClient _slackClient;
        private readonly SlackMessagesStorage _storage;

        public LinkStatsHandler(IEnumerable<IPublisher> publishers, ISlackClient slackClient, SlackMessagesStorage storage)
        {
            _publishers = publishers;
            _slackClient = slackClient;
            _storage = storage;
        }

        public bool ShouldShowInHelp => true;

        public Tuple<string, string> GetHelpDescription() => new Tuple<string, string>("linkstats @username", "Statistikk på delte lenker for en gitt smarting");

        public async Task<HandleResponse> Handle(SlackMessage message)
        {
            var strings = message.Text.Split(" ");
            if (strings.Length >= 3)
            {
                var users = await _slackClient.UsersList();
                var parsedName = strings[2];
                var usr = users.Members.FirstOrDefault(u => $"<@{u.Id}>" == parsedName);
                var smarting = parsedName;
                
                if (usr != null)
                {
                    var urlsForSmarting = await _storage.GetAllUrlsByUser(usr.Name);
                    var domains = urlsForSmarting
                        .Select(c => new Uri(c.Url))
                        .GroupBy(u => u.Host);

                    var text = $"!!{smarting}:\n";
            
                    if (!domains.Any())
                    {
                        var actual = await _storage.GetUniqueUsersForUrls();
                        text += $"Fant ikke {smarting}. Prøv med en av {string.Join(',', actual)}";
                    }

                    domains = domains.Where(o => o.Count() > 1).OrderByDescending(o => o.Count()).Take(5);

                    if (!domains.Any())
                    {
                        text += "Har ikke delt NOE fra samme domene 2 ganger! Jikes.";
                    }
                
                    foreach (var domain in domains)
                    {
                        text += $"\n•_{domain.Key}_ - {domain.Count()}";
                    } 
                    
                    foreach (var publisher in _publishers)
                    {
                        await publisher.Publish(new Notification
                        {
                            Recipient = message.ChatHub.Id,
                            Msg = text
                        });
                    }
                }

            }
            else
            {
                var actual = await _storage.GetUniqueUsersForUrls();
                var text = $"Mangla litt input. Prøv `linkstats` og en av disse som input: {string.Join(',', actual)}";
                foreach (var publisher in _publishers)
                {
                    await publisher.Publish(new Notification
                    {
                        Recipient = message.ChatHub.Id,
                        Msg = text
                    });
                }
            }

            return new HandleResponse("OK");
        }

        public bool ShouldHandle(SlackMessage message)
        {
            var sharedUrls = message.Text.Contains("linkstats", StringComparison.InvariantCultureIgnoreCase);
            return message.MentionsBot && sharedUrls;        
        }
    }
}