using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Network
{
    public static class NetworkOptions
    {
        public static string Ip { get; }= "192.168.1.66";
        public static int Port { get; } = 5000;
        public static int MaxPlayers { get; } = 2;
    }
}
