using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Xml;

namespace WebApplication1.Models
{
    [Serializable]
    public class WeChatMessage
    {
        public enum MessageType
        {
            /// <summary>
            /// 文本消息
            /// </summary>
            text,
            /// <summary>
            /// 图片
            /// </summary>
            image,
            /// <summary>
            /// 语音消息
            /// </summary>
            voice,
            /// <summary>
            /// 视频消息
            /// </summary>
            video,
            /// <summary>
            /// 短视频消息
            /// </summary>
            shortvideo,
            /// <summary>
            /// 定位消息
            /// </summary>
            location,
            /// <summary>
            /// 超链接跳转事件
            /// </summary>
            @link,
            /// <summary>
            /// 菜单点击事件
            /// </summary>
            @event
        }

        [JsonProperty(Order = -100)]
        /// <summary>
        /// 开发者 微信号
        /// </summary>
        public string ToUserName { get; set; }

        [JsonProperty(Order = -99)]
        /// <summary>
        /// 发送方帐号（一个OpenID）
        /// </summary>
        public string FromUserName { get; set; }

        [JsonProperty(Order = -98)]
        /// <summary>
        /// 消息创建时间 （整型）
        /// </summary>
        public string CreateTime { get; set; }

        [JsonProperty(Order = -98)]
        [JsonConverter(typeof(StringEnumConverter))]
        /// <summary>
        /// 消息类型，event
        /// </summary>
        public MessageType MsgType { get; set; }

        public static T Load<T>(string xmlString) where T : WeChatMessage
        {
            ///正则表达式 去除xml中 CDATA属性标签
            xmlString = xmlString.Replace("<![CDATA[", "").Replace("]]>", "");
            ///加载xml
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);
            ///xml转换成json
            string jsonString = JsonConvert.SerializeXmlNode(doc);
            ///反序列化json
            XmlRoot<T> xmlroot = JsonConvert.DeserializeObject<XmlRoot<T>>(jsonString);
            ///返回根节点信息
            return xmlroot.xml;
        }

        /// <summary>
        /// 序列化为xml字符串
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string SerializeXml(object model)
        {
            ///按照接口文档字段进行格式封装
            object xmlModel = new { xml = model };
            string jsonString = JsonConvert.SerializeObject(xmlModel);
            XmlDocument xml = JsonConvert.DeserializeXmlNode(jsonString);
            return xml.InnerXml;
        }

        /// <summary>
        /// 根节点信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        class XmlRoot<T> where T : WeChatMessage
        {
            /// <summary>
            /// 根节点
            /// </summary>
            public T xml { get; set; }
        }
    }

    public class EventMessage : WeChatMessage
    {
        public enum EventType
        {
            subscribe,
            unsubscribe,
            SCAN
        }

        public EventType Event { get; set; }
    }

    public class ParameterizedQRCodeScannedEventMessage : EventMessage
    {
        public string EventKey { get; set; }
        public string Ticket { get; set; }
        
    }
}
