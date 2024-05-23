using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
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
            InitializeTwitterClient();
        }

        private void InitializeTwitterClient()
        {
            var apiKey = _configuration["TwitterApi:ApiKey"];
            var apiSecretKey = _configuration["TwitterApi:ApiSecretKey"];
            var accessToken = _configuration["TwitterApi:AccessToken"];
            var accessTokenSecret = _configuration["TwitterApi:AccessTokenSecret"];

            var client = new TwitterClient(apiKey, apiSecretKey, accessToken, accessTokenSecret);
            TweetinviConfig.CurrentThreadSettings.TwitterClient = client;
        }

        public async Task<bool> TweetBlogPostAsync(string message)
        {
            try
            {
                var tweet = await TweetinviConfig.CurrentThreadSettings.TwitterClient.Tweets.PublishTweetAsync(new PublishTweetParameters(message));
                return tweet != null;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Failed to tweet message. Exception: {ex.Message}");
                return false;
            }
        }
    }
}
