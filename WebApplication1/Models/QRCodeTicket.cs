using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    public class QRCodeTicket
    {
        [JsonProperty("ticket")]
        public string Ticket { get; set; }
        [JsonProperty("expire_seconds")]
        public int ExpireSeconds { get; set; }
        [JsonProperty("url")]
        public string URL { get; set; }
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Scene {
        [JsonProperty("scene_str")]
        public string? SceneStr { get; set; }

        [JsonProperty("scene_id")]
        public int? SceneId { get; set; } 
    }
    public class ActionInfo {
        [JsonProperty("scene")]
        public Scene Scene { get; set; }
    }

    public class QRCodeTicketRequestBody { 
        [JsonProperty("expire_seconds")]
        public int ExpireSeconds { get; set; }

        [JsonProperty("action_name")]
        public string ActionName { get; set; }
        [JsonProperty("action_info")]
        public ActionInfo ActionInfo { get; set; }
    }
}
