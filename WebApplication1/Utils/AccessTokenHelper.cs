using StackExchange.Redis;
using System.Diagnostics.Eventing.Reader;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApplication1.Utils
{
    public class AccessTokenHelper
    {
        private readonly IDatabase _redis;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        class TokenResponse
        {
            [JsonPropertyName("access_token")]
            public string? AccessToken { get; set; }
            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }

        }
        public AccessTokenHelper(RedisHelper redis,IConfiguration configuration,HttpClient httpClient)
        {
            _redis = redis.GetDatabase();
            _configuration = configuration;
            _httpClient = httpClient;
        }
        public  async Task<string> FetchAccessToken()
        {
            var accessToken = await GetAccessTokenFromRedis();

            if (string.IsNullOrEmpty(accessToken))
            {
                var tokenResp = await GetAccessTokenFromWechat();
                await SaveAccessTokenToRedis(tokenResp);
                accessToken = tokenResp.AccessToken;
            }

            return accessToken;
            
        }

        private string Sha1Sign(string str)
        {
            HashAlgorithm sha1 = new SHA1CryptoServiceProvider();
            byte[] hashed_bytes = sha1.ComputeHash(Encoding.Default.GetBytes(str));
            string res = BitConverter.ToString(hashed_bytes);

            return res;

        }

        public bool CheckSignature(string signature, string timestamp, string nonce)
        {
            var token = _configuration["WeChat:Token"];
            string[] strs = { token,timestamp,nonce};
            Array.Sort(strs);
            var joinedStr = String.Join("", strs);
            var hashedStr = Sha1Sign(joinedStr);
            hashedStr = hashedStr.Replace("-","").ToLower();

            if (signature == hashedStr)
                return true;
            return false;

        }

        private  async Task<string> GetAccessTokenFromRedis()
        {
            var token = await _redis.StringGetAsync("access_token");
            return token;
        }
        private async Task SaveAccessTokenToRedis(TokenResponse resp)
        {
            var expiresIn = new TimeSpan(0, 0, resp.ExpiresIn);
            await _redis.StringSetAsync("access_token", resp.AccessToken, expiresIn);
        }

        private async Task<TokenResponse> GetAccessTokenFromWechat()
        {
            var baseUrl = _configuration["WeChat:BaseUrl"];
            var accessTokenUrl = _configuration["WeChat:AccessTokenUrl"];
            var appid = _configuration["WeChat:appID"];
            var secret = _configuration["WeChat:appsecret"];

            var url = baseUrl + accessTokenUrl + "&appid=" + appid + "&secret=" + secret;

            var resp = await _httpClient.GetAsync(url);

            resp.EnsureSuccessStatusCode();

            var content = await resp.Content.ReadAsStringAsync();
            var tokenResp = JsonSerializer.Deserialize<TokenResponse>(content);
            return tokenResp;
        }
    }
}
