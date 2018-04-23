using System;
using System.Text;
using System.Web.Script.Serialization;
using System.Collections.Generic;

namespace WeChatService
{
    public class Process
    {
        JavaScriptSerializer convert = new JavaScriptSerializer();
        public event Action<IntPtr, object> ReceiveMessage;
        Service service;

        public Process(Service service)
        {
            this.service = service;
        }

        void OnReceiveMessage(IntPtr connId, object msg)
        {
            if (this.ReceiveMessage != null)
                this.ReceiveMessage(connId, msg);
        }

        int ArrayCopy(Array sourceArray, int sourceIndex, Array destinationArray, int destinationIndex)
        {
            int rLength = destinationArray.Length - destinationIndex, aLength = sourceArray.Length - sourceIndex;
            int length = rLength < aLength ? rLength : aLength;
            Array.Copy(sourceArray, sourceIndex, destinationArray, destinationIndex, length);
            return length;
        }

        public void RecvData(IntPtr connId, ExtraData msg, byte[] bytes)
        {
            int position = 0, length = 0;
            while (position < bytes.Length)
            {
                if (msg.Data == null)
                {
                    length = ArrayCopy(bytes, position, msg.Head, msg.Position);
                    msg.Position += length;
                    if (msg.Position == msg.Head.Length)
                    {
                        msg.Data = new byte[GetHead(msg.Head)];
                        msg.Position = 0;
                    }
                }
                else
                {
                    length = ArrayCopy(bytes, position, msg.Data, msg.Position);
                    msg.Position += length;
                    if (msg.Position == msg.Data.Length)
                    {
                        var obj = FormatterBytesObject(msg.Data);
                        if (!msg.Access)
                        {
                            var dic = obj as Dictionary<string, object>;
                            if (dic != null)
                                if (msg.GUID == dic["key"].ToString())
                                {
                                    msg.Access = true;
                                    this.service.SendMessage(connId, StateList.State[StateInfo.Authorized]);
                                }
                        }
                        if (msg.Access)
                            this.OnReceiveMessage(connId, obj);
                        else
                        {
                            this.service.SendMessage(connId, StateList.State[StateInfo.Unauthorized]);
                            this.service.Disconnect(connId);
                        }
                        msg.Data = null;
                        msg.Position = 0;
                    }
                }
                position += length;
            }
        }

        int GetHead(byte[] bytes)
        {
            return BitConverter.ToInt32(bytes, 0);
        }

        byte[] SetHead(byte[] bytes)
        {
            byte[] bHead = BitConverter.GetBytes(bytes.Length);
            byte[] bRst = new byte[bytes.Length + 4];
            Array.Copy(bHead, 0, bRst, 0, bHead.Length);
            Array.Copy(bytes, 0, bRst, bHead.Length, bytes.Length);
            return bRst;
        }

        public byte[] FormatterMessageBytes(object message)
        {
            return SetHead(FormatterObjectBytes(message));
        }

        public object FormatterBytesObject(byte[] buff)
        {
            return FormatterBytesObject<object>(buff);
        }

        public T FormatterBytesObject<T>(byte[] buff)
        {
            return convert.Deserialize<T>(Encoding.Default.GetString(buff));
        }

        byte[] FormatterObjectBytes(object obj)
        {
            return Encoding.Default.GetBytes(convert.Serialize(obj));
        }
    }
}
