using Newtonsoft.Json;

namespace WebApplication1.Models
{
    public class WeChatUserBasicInfo
    {
        [JsonProperty("subscribe")]
        public int Subscribe { get; set; }

        [JsonProperty("openid")]
        public string OpenId { get; set; }

        [JsonProperty("nickname")]
        public string NickName { get; set; }

        [JsonProperty("sex")]
        public int Sex { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("province")]
        public string Province { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("headimgurl")]
        public string HeadImgUrl { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("subscribe_time")]
        public Int64 SubscribeTime { get; set; }

        [JsonProperty("unionid")]
        public string UnionId { get; set; }

        [JsonProperty("remark")]
        public string Remark { get; set; }

        [JsonProperty("groupid")]
        public int GroupId { get; set; }

        [JsonProperty("tagid_list")]
        public List<int> TagIdList { get; set; }

        [JsonProperty("subscribe_scene")]
        public string SubcribeScene { get; set; }

        [JsonProperty("qr_scene")]
        public int QRScene { get; set; }

        [JsonProperty("qr_scene_str")]
        public string QRSceneStr { get; set; }
    }
}
