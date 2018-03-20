using System;

namespace QueueMessage
{
    public enum MessageType { Result, Restart, Login, Call, Rate, Operate, Logout }
    public enum ClientType { Window, SoundPlayer, LEDDisplay, ScreenDisplay }

    [Serializable]
    public class Message
    {
        public MessageType Type { get; set; }

        public override string ToString()
        {
            return "基础消息类型";
        }
    }

    [Serializable]
    public class ResultMessage : Message
    {
        public ResultMessage()
        {
            this.Type = MessageType.Result;
        }
        public MessageType Operate { get; set; }
        public string Result { get; set; }
        public override string ToString()
        {
            return string.Format("操作返回消息:Operate={0} Result={1}", this.Operate, this.Result);
        }
    }

    [Serializable]
    public class LoginMessage : Message
    {
        public LoginMessage()
        {
            this.Type = MessageType.Login;
        }
        public ClientType ClientType { get; set; }
        public string ClientName { get; set; }
    }

    [Serializable]
    public class CallMessage : Message
    {
        public CallMessage()
        {
            this.Type = MessageType.Call;
        }

        public bool IsSoundMessage { get; set; }

        public bool IsLEDMessage { get; set; }

        public string AreaNo { get; set; }

        public string BusinessNo { get; set; }

        public string WindowNo { get; set; }

        public string TicketNo { get; set; }

        public override string ToString()
        {
            return string.Format("请{0}号到第{1}窗口办理业务", this.TicketNo, this.WindowNo);
        }
    }

    [Serializable]
    public class RateMessage : Message
    {
        public RateMessage()
        {
            this.Type = MessageType.Rate;
        }
        /// <summary>
        /// 评价流水号
        /// </summary>
        public string RateId { get; set; }
        /// <summary>
        ///  窗口号
        /// </summary>
        public string WindowNo { get; set; }
        /// <summary>
        /// 办理人
        /// </summary>
        public string Transactor { get; set; }
        /// <summary>
        /// 办理日期
        /// </summary>
        public string WorkDate { get; set; }
        /// <summary>
        /// 办理事项
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 预约号
        /// </summary>
        public string reserveSeq { get; set; }

        public override string ToString()
        {
            return string.Format("请求评价消息:RateId={0} WindowNo={1}", this.RateId, this.WindowNo);
        }
    }

    public enum Operate { Pause, Resume, Reset }

    [Serializable]
    public class OperateMessage : Message
    {
        public OperateMessage()
        {
            this.Type = MessageType.Operate;
        }

        /// <summary>
        ///  窗口号
        /// </summary>
        public string WindowNo { get; set; }

        public Operate Operate { get; set; }

        public override string ToString()
        {
            return string.Format("业务操作消息:Operate={0} WindowNo={1}", this.Operate, this.WindowNo);
        }
    }

    [Serializable]
    public class LogoutMessage : Message
    {
        public LogoutMessage()
        {
            this.Type = MessageType.Logout;
        }
    }
}
