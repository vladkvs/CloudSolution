using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using DotNetOpenAuth.OpenId.Extensions.OAuth;

namespace CloudSolution.FourShared
{
    class TokenManager : IConsumerTokenManager
    {
        private static Dictionary<string, string> TokenSecrets =
            new Dictionary<string, string>();

        public TokenManager(string consumerKey, string consumerSecret)
        {
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
        }

        public string ConsumerKey { get; private set; }

        public string ConsumerSecret { get; private set; }

        public string GetTokenSecret(string token)
        {
            return TokenSecrets[token];
        }

        public void StoreNewRequestToken(UnauthorizedTokenRequest request,
            ITokenSecretContainingMessage response)
        {
            TokenSecrets[response.Token] = response.TokenSecret;
        }

        public void ExpireRequestTokenAndStoreNewAccessToken(
            string consumerKey,
            string requestToken,
            string accessToken,
            string accessTokenSecret)
        {
            TokenSecrets.Remove(requestToken);
            TokenSecrets[accessToken] = accessTokenSecret;
        }

        public TokenType GetTokenType(string token)
        {
            throw new NotImplementedException();
        }

        public void StoreOpenIdAuthorizedRequestToken(string consumerKey,
            AuthorizationApprovedResponse authorization)
        {
            TokenSecrets[authorization.RequestToken] = String.Empty;
        }
    }
}
