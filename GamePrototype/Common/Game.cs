using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Common
{
    public class Game : IDisposable
    {
        public delegate void OnGameStateChangeEventHandler(GameModes mode);
        public event OnGameStateChangeEventHandler GameStateChangeEvent;

        public Dictionary<Player, Task> PlayerList { get; set; }
        private Dictionary<Player, Task>.Enumerator enumerator;
        public List<Player> AttackOrder { get; set; } = new List<Player>(); 
        public int TargetNumber { get; set; }
        public List<Move> MoveList { get; set; }

        public void OnGameStateChangeEvent(GameModes mode)
        {
            GameStateChangeEvent?.Invoke(mode);
            enumerator = PlayerList.GetEnumerator();
        }

        public Player GetPlayerFromIpEndPoint(IPEndPoint endPoint)
        {
            foreach (var player in PlayerList)
            {
                if (Equals(player.Key.Client.Client.RemoteEndPoint, endPoint))
                {
                    return player.Key;    
                }
            }
            return null;
        }

        public Player GetPlayerByName(string name)
        {
            while (true)
            {
                if (enumerator.Current.Key.PlayerName == name)
                {
                    var p1 = enumerator.Current.Key;
                    enumerator.Dispose();
                    return p1;
                }
                enumerator.MoveNext();
            }
        }

        public Player GetPlayerByTcpClient(TcpClient client)
        {
            while (true)
            {
                if (enumerator.Current.Key.Client.Equals(client))
                {
                    var p1 = enumerator.Current.Key;
                    enumerator.Dispose();
                    return p1;
                }
                enumerator.MoveNext();
            }
        }

        public Player GetPlayer(Player p)
        {
            while (true)
            {
                if (enumerator.Current.Key.Client.Equals(p.Client))
                {
                    var p1 = enumerator.Current.Key;
                    enumerator.Dispose();
                    return p1;
                }
            }
        }

        public void Dispose()
        {
            enumerator.Dispose();
        }
    }
}
