using Microsoft.Extensions.Options;
using Osta.Identity.ExternalLogin.FaceBook;
using Osta.Identity.Models;
using System.Text.Json;

namespace Osta.Identity.ExternalLogin.Facebook
{
    public class FacebookAuthService : IFacebookAuthService
    {
        private readonly FaceBookModelConfiguration _faceBookModelConfiguration;
        private readonly HttpClient httpClient;
        private readonly string appId;
        private readonly string appSecret;

        public FacebookAuthService(HttpClient httpClient, IOptions<FaceBookModelConfiguration> options)
        {
            _faceBookModelConfiguration = options.Value;
            this.httpClient = httpClient;
            appId = _faceBookModelConfiguration.AppId;
            appSecret = _faceBookModelConfiguration.AppSecret;

        }

        public async Task<FacebookUserModel> ValidateAccessTokenAsync(string accessToken)
        {

            var appAccessToken = $"{appId}|{appSecret}";
            var debugUrl = $"https://graph.facebook.com/debug_token?input_token={accessToken}&access_token={appAccessToken}";

            var debugResponse = await httpClient.GetAsync(debugUrl);
            if (!debugResponse.IsSuccessStatusCode)
                throw new UnauthorizedAccessException("Invalid Facebook token");

            var debugContent = await debugResponse.Content.ReadAsStringAsync();
            using var debugJson = JsonDocument.Parse(debugContent);
            var data = debugJson.RootElement.GetProperty("data");
            var isValid = data.GetProperty("is_valid").GetBoolean();
            var tokenAppId = data.GetProperty("app_id").GetString();

            if (!isValid || tokenAppId != appId)
                throw new UnauthorizedAccessException("Invalid Facebook token");


            var userInfoUrl = $"https://graph.facebook.com/me?fields=id,name,email,picture&access_token={accessToken}";
            var userResponse = await httpClient.GetAsync(userInfoUrl);

            if (!userResponse.IsSuccessStatusCode)
                throw new UnauthorizedAccessException("Failed to fetch Facebook user info");

            var userContent = await userResponse.Content.ReadAsStringAsync();
            using var userJson = JsonDocument.Parse(userContent);
            var root = userJson.RootElement;

            return new FacebookUserModel
            {
                Id = root.GetProperty("id").GetString(),
                Name = root.GetProperty("name").GetString(),
                Email = root.TryGetProperty("email", out var email) ? email.GetString() : null,
                PictureUrl = root.TryGetProperty("picture", out var pic)
                    ? pic.GetProperty("data").GetProperty("url").GetString()
                    : null
            };
        }
    }
}