using System.Text;
using System.Web.Script.Serialization;

namespace RateService
{

    public class MessageData
    {
        protected JavaScriptSerializer convert = new JavaScriptSerializer();
        public MessageData()
        {
        }

        public virtual byte[] ToResultData()
        {
            return Encoding.UTF8.GetBytes(convert.Serialize(this));
        }

    }


    public class RequestData : MessageData
    {
        public string method { get; set; }
        public object param { get; set; }
    }

    public class ResponseData : MessageData
    {
        public string request { get; set; }
        public string code { get; set; }
        public object result { get; set; }
    }
}
