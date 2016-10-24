using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class StringMessage : Message<String> 
    {
        
        public StringMessage(string data, IPEndPoint sender)
        {
            Data = data;
            Sender = sender;
        }
    }
}
