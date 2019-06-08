using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JorgBot.Publishers.Slack
{
    public class SlackChannels
    {
        public SlackChannels(IHostingEnvironment env, ILogger<SlackChannels> logger)
        {
            if (env.IsDevelopment())
            {
                logger.LogInformation("Development. Using testchannel");
                BursdagerChannel = TestChannelId;
                SmartebokaChannel = TestChannelId;
                JorgChannel = TestChannelId;
            }
            else
            {
                logger.LogInformation("Production. Using prod channels");
                BursdagerChannel = BursdagerChannelId;
                SmartebokaChannel = SmartebokaChannelId;
                JorgChannel = JorgChannelId;
            }
        }

        /// <summary>
        /// https://smarteboka.slack.com/messages/CGY1XJRM1/details/
        /// </summary>
        public const string TestChannelId = "CGY1XJRM1";
        public const string BursdagerChannelId = "CK1TE2NN6";
        public const string SmartebokaChannelId = "C0EC3DG5N";
        public const string JorgChannelId = "CKDDD3MLM";

        public string BursdagerChannel
        {
            get;
            private set;
        }
        
        public string SmartebokaChannel
        {
            get;
            private set;
        }
        
        public string JorgChannel
        {
            get;
            private set;
        }
    }
}