using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;

namespace CloudSolution.FourShared
{
    class FourSharedConsumer
    {
        private string _requestToken = string.Empty;

        public DesktopConsumer Consumer { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }

        public FourSharedConsumer(string consumerKey, string consumerSecret)
        {
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;

            var providerDescription = new ServiceProviderDescription
            {
                RequestTokenEndpoint = new MessageReceivingEndpoint(
                    "https://api.4shared.com/v1_2/oauth/initiate",
                    HttpDeliveryMethods.PostRequest),
                UserAuthorizationEndpoint = new MessageReceivingEndpoint(
                    "https://api.4shared.com/v1_2/oauth/authorize",
                    HttpDeliveryMethods.GetRequest),
                AccessTokenEndpoint = new MessageReceivingEndpoint(
                    "https://api.4shared.com/v1_2/oauth/token",
                    HttpDeliveryMethods.PostRequest),
                TamperProtectionElements = new ITamperProtectionChannelBindingElement[]
                {
                    new HmacSha1SigningBindingElement(),
                }
            };

            Consumer = new DesktopConsumer(
                providerDescription,
                new TokenManager(ConsumerKey, ConsumerSecret));
        }

        public Uri BeginAuth()
        {
            var requestArgs = new Dictionary<string, string>();
            var request = Consumer
                .RequestUserAuthorization(requestArgs, null, out _requestToken);
            return request;
        }

        public string CompleteAuth(string verifier)
        {
            var response = Consumer.ProcessUserAuthorization(
                _requestToken, verifier);
            return response.AccessToken;
        }

        public HttpWebRequest PrepareAuthorizedRequest(
            MessageReceivingEndpoint endpoint,
            string accessToken,
            IEnumerable<MultipartPostPart> parts)
        {
            return Consumer.PrepareAuthorizedRequest(endpoint, accessToken, parts);
        }

        public IConsumerTokenManager TokenManager => Consumer.TokenManager;
    }
}
