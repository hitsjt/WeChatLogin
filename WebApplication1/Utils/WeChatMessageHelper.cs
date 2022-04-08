using Newtonsoft.Json;
using StackExchange.Redis;
using System.Xml;
using System.Xml.Serialization;
using WebApplication1.Models;

namespace WebApplication1.Utils
{
    public class WeChatMessageHelper
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IDatabase _redis;
        public WeChatMessageHelper(RedisHelper redis,IConfiguration configuration,HttpClient httpClient)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _redis = redis.GetDatabase();
            
        }
        public async Task ProcessMessage(string xmlMessage,string accessToken)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlMessage);

            var messageType = doc.GetElementsByTagName("MsgType")[0].FirstChild.Value;

            if (messageType == "event")
                await ProcessParameterizedQRCodeScannedEventMessage(xmlMessage, accessToken);
        }

        private async Task ProcessParameterizedQRCodeScannedEventMessage(string xmlMessage,string accessToken)
        {
            var messageObj = WeChatMessage.Load<ParameterizedQRCodeScannedEventMessage>(xmlMessage);

            if (messageObj.Event == EventMessage.EventType.subscribe)
            {
                await ProcessSubscribeInParameterizedQRCodeScannedEvent(messageObj,accessToken);

            }
            else if (messageObj.Event == EventMessage.EventType.SCAN)
            {
                Console.WriteLine("hahah");
            }
        }

        private async Task ProcessSubscribeInParameterizedQRCodeScannedEvent(ParameterizedQRCodeScannedEventMessage message, string accessToken)
        {
            var userId = message.FromUserName;
            var eventKey = message.EventKey;
            var ticket = message.Ticket;

            var baseUrl = _configuration["WeChat:BaseUrl"];
            var userInfoUrl = _configuration["WeChat:UserInfoUrl"];
            var language = _configuration["WeChat:UserInfoLanguage"];
            var url = baseUrl + userInfoUrl + "access_token=" + accessToken + "&openid=" + userId + "&lang=" + language;

            var resp = await _httpClient.GetAsync(url);
            resp.EnsureSuccessStatusCode();

            var content = await resp.Content.ReadAsStringAsync();

            var userInfo = JsonConvert.DeserializeObject<WeChatUserBasicInfo>(content);
            
            var qrCodeParam = message.EventKey.Substring(8);
            List<HashEntry> hashEntries = new List<HashEntry>();

            hashEntries.Add(new HashEntry("openid", userInfo.OpenId));
            hashEntries.Add(new HashEntry("unionid", string.IsNullOrEmpty(userInfo.UnionId)?"": userInfo.UnionId));

            _redis.HashSetAsync(qrCodeParam, hashEntries.ToArray());

            Console.WriteLine(qrCodeParam);

        }
    }
}
