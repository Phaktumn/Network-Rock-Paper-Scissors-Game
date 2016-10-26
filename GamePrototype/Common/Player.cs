using System.Net;
using System.Net.Sockets;

namespace Common
{
    public class Player
    {
        public delegate void OnAttackEventHandler();
        public event OnAttackEventHandler AttackEvent;

        public int Id { get; set; }
        public string PlayerName { get; set; }
        public IPEndPoint PlayerAddress { get; set; }
        public TcpClient Client { get; set; }
        public int Port { get; set; }
    }
}
