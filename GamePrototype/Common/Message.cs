using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common.Network;

namespace Common
{
    public class Message
    {
        public IPEndPoint Sender { get; set; }
        public string Data { get; set; }
        public CodeMessages.NetworkCode Code { get; set; }

        public Message(string data, IPEndPoint sender)
        {
            this.Data = data;
            this.Sender = sender;
        }

        public Message(CodeMessages.NetworkCode code, IPEndPoint sender)
        {
            this.Code = code;
            this.Sender = sender;
        }

        public static string[] Deserialize(Message packet)
        {
            return packet.Data.Split(CodeMessages.MESSAGE_SPLITER_CODE.ToCharArray());
        }
    }
}
