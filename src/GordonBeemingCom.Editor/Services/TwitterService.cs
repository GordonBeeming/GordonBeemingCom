using Microsoft.Extensions.Configuration;
using Tweetinvi;
using Tweetinvi.Parameters;

namespace GordonBeemingCom.Editor.Services
{
    public class TwitterService
    {
        private readonly IConfiguration _configuration;

        public TwitterService(IConfiguration configuration)
        {
            _configuration = configuration;
            InitializeClient();
        }

        private void InitializeClient()
        {
            var consumerKey = _configuration["TwitterAPI:ApiKey"];
            var consumerSecret = _configuration["TwitterAPI:ApiSecretKey"];
            var accessToken = _configuration["TwitterAPI:AccessToken"];
            var accessTokenSecret = _configuration["TwitterAPI:AccessTokenSecret"];

            var userClient = new TwitterClient(consumerKey, consumerSecret, accessToken, accessTokenSecret);
            TweetinviConfig.CurrentThreadSettings.TwitterClient = userClient;
        }

        public async Task<bool> TweetBlogPostAsync(string message, string url)
        {
            var tweet = await TweetinviConfig.CurrentThreadSettings.TwitterClient.Tweets.PublishTweetAsync(new PublishTweetParameters($"{message} {url}"));
            return tweet != null;
        }
    }
}
