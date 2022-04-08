using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Xml;
using WebApplication1.Utils;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {


        private readonly IConfiguration _configuration;
        private readonly IDatabase _redis;
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly AccessTokenHelper _tokenHelper;
        private readonly QRCodeHelper _qrCodeHelper;
        private readonly WeChatMessageHelper _weChatMessageHelper;


        public AuthController(IConfiguration configuration, RedisHelper redis)
        {
            _configuration = configuration;
            _redis = redis.GetDatabase();

            _httpClient.Timeout = new TimeSpan(0, 0, 30);


            _tokenHelper = new AccessTokenHelper(redis, configuration,_httpClient);
            _qrCodeHelper = new QRCodeHelper(_configuration, _httpClient);
            _weChatMessageHelper = new WeChatMessageHelper(redis,_configuration, _httpClient );


        }

        [HttpGet]
        public string Get()
        {
            var accessToken = 
            // 往Redis里面存入数据
            _redis.StringSet("Name", "Tom");
            // 从Redis里面取数据
            //string name = _redis.StringGet("Name");
            return "aaa";
        }

        [HttpGet("QRCode")]
        public async Task<IActionResult> GetQRCode(string userId,int type)
        {
            var accessToken = await _tokenHelper.FetchAccessToken();

            var qrCodeTicket = await _qrCodeHelper.GetQRCode(accessToken, userId);

            var based64EncodedQRCode = await _qrCodeHelper.GetQRCodeFromWeChat(qrCodeTicket);

            

            return Ok(based64EncodedQRCode);
        }
        [HttpGet("Event")]
        public IActionResult GetEvent(string signature, string timestamp, string nonce, string echostr)
        {
            if(_tokenHelper.CheckSignature(signature,timestamp,nonce))
                return Ok(echostr);
            else return BadRequest("Failed");
        }

        [HttpPost("Event")]
        public async Task<IActionResult> PostEvent()
        {
            var str = string.Empty;
            using (var stream = Request.Body)
            {
                using (var streamReader = new StreamReader(stream))
                {
                    str = await streamReader.ReadToEndAsync();
                }
            }

            var accessToken = await _tokenHelper.FetchAccessToken();

            await _weChatMessageHelper.ProcessMessage(str, accessToken);


            return Ok();

        }



        //private async string GetAccessToken()
        //{ 

        //    return ""
        //}


    }
}
