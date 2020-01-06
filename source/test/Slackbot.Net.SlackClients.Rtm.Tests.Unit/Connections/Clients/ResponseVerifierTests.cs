﻿using Shouldly;
using Slackbot.Net.SlackClients.Rtm.Connections.Clients;
using Slackbot.Net.SlackClients.Rtm.Connections.Responses;
using Slackbot.Net.SlackClients.Rtm.Exceptions;
using Xunit;

namespace Slackbot.Net.SlackClients.Rtm.Tests.Unit.Connections.Clients
{
    public class ResponseVerifierTests
    {
        [Fact]
        public void should_throw_exception_with_given_error_message_when_request_failed()
        {
            // given
            var response = new StandardResponse { Ok = false, Error = "I AM A ERROR-message" };
            var verifier = new ResponseVerifier();

            // when && then
            var exception = Assert.Throws<CommunicationException>(() => verifier.VerifyResponse(response));
            exception.Message.ShouldBe($"Error occured while posting message '{response.Error}'");
        }

        [Fact]
        public void should_not_throw_exception()
        {
            // given
            var response = new StandardResponse { Ok = true };
            var verifier = new ResponseVerifier();

            // when && then
            verifier.VerifyResponse(response);
        }
    }
}