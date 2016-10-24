using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common.Network
{
    class TcpObserver
    {
        public delegate void OnMessageReceivedEventHandler(Message packet);
        public delegate void OnClientConnectedEventHandler(IPEndPoint address);
        public event OnMessageReceivedEventHandler MessageReceivedEvent;
        public event OnClientConnectedEventHandler ClientConnectedEvent;

        private TcpListener listener { get; set; }
        private Task awaitConnections;


        public TcpObserver()
        {

        }

        public void Connect()
        {
            //listener.
        }

        public void Read()
        {

        }

        protected virtual void OnMessageReceivedEvent(Message packet)
        {
            MessageReceivedEvent?.Invoke(packet);
        }

        protected virtual void OnClientConnectedEvent(IPEndPoint address)
        {
            ClientConnectedEvent?.Invoke(address);

        }
    }
}
