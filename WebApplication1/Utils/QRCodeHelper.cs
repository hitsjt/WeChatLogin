using Microsoft.AspNetCore.Routing.Tree;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.Utils
{
    public class QRCodeHelper
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public QRCodeHelper(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GetQRCode(string accessToken, string sceneStr)
        {
            var qrCodeTicket = await GetQRCodeTicketFromWeChat(accessToken, sceneStr);

            return qrCodeTicket.Ticket;

        }

        public async Task<string> GetQRCodeFromWeChat(string qrCodeTicket)
        {
            var encodedTicket = HttpUtility.UrlEncode(qrCodeTicket);
            var url = _configuration["WeChat:ShowQRCode"] + encodedTicket;

            var resp = await _httpClient.GetAsync(url);

            resp.EnsureSuccessStatusCode();

            var content = await resp.Content.ReadAsByteArrayAsync();
            var base64EncodedContent = Convert.ToBase64String(content);
            return base64EncodedContent;
        }



        private async Task<QRCodeTicket> GetQRCodeTicketFromWeChat(string accessToken,string sceneStr)
        {
            var baseUrl = _configuration["WeChat:BaseUrl"];
            var createQRCodeUrl = _configuration["WeChat:CreateQRCodeUrl"];
            var url = baseUrl + createQRCodeUrl + "access_token=" + accessToken;

            var qrCodeTicketRequestBody = new QRCodeTicketRequestBody();
            qrCodeTicketRequestBody.ExpireSeconds = 604800;
            qrCodeTicketRequestBody.ActionName = "QR_STR_SCENE";
            qrCodeTicketRequestBody.ActionInfo = new ActionInfo();
            qrCodeTicketRequestBody.ActionInfo.Scene = new Scene();
            qrCodeTicketRequestBody.ActionInfo.Scene.SceneStr = sceneStr;

           var serializedRequestBody =  JsonConvert.SerializeObject(qrCodeTicketRequestBody);


            var httpStringContent = new StringContent(serializedRequestBody);

            var resp = await _httpClient.PostAsync(url, httpStringContent);

            resp.EnsureSuccessStatusCode();

            var content = await resp.Content.ReadAsStringAsync();
            var qrCodeTicket = JsonConvert.DeserializeObject<QRCodeTicket>(content);
            
            return qrCodeTicket;
        }





    }
}
