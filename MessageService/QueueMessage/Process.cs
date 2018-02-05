using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace QueueMessage
{
    public class Process
    {
        public Process()
        {
        }

        public event Action<IntPtr, Message> ReceiveMessage;

        void OnReceiveMessage(IntPtr connId, Message msg)
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
                        this.OnReceiveMessage(connId, FormatterByteObject(msg.Data) as Message);
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

        public byte[] FormatterMessageBytes(Message message)
        {
            return SetHead(FormatterObjectBytes(message));
        }

        byte[] FormatterObjectBytes(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj is null");
            byte[] buff;
            using (var ms = new MemoryStream())
            {
                IFormatter iFormatter = new BinaryFormatter();
                iFormatter.Serialize(ms, obj);
                buff = ms.GetBuffer();
            }
            return buff;
        }

        object FormatterByteObject(byte[] buff)
        {
            if (buff == null)
                throw new ArgumentNullException("buff is null");
            object obj;
            using (var ms = new MemoryStream(buff))
            {
                IFormatter iFormatter = new BinaryFormatter();
                obj = iFormatter.Deserialize(ms);
            }
            return obj;
        }
    }
}
