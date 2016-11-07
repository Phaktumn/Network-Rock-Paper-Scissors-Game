using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common.Network
{
    public class TcpObserver
    {
        public delegate void OnMessageReceivedEventHandler(Message packet);
        public delegate void OnClientConnectedEventHandler(IPEndPoint address);
        public event OnMessageReceivedEventHandler MessageReceivedEvent;
        public event OnClientConnectedEventHandler ClientConnectedEvent;

        public TcpListener Listener { get { return listener; } }

        private TcpListener listener { get; set; }
        private Task awaitConnections;

        public bool ShutDown;

        /// <summary>
        /// time in ms
        /// 1000 = 1 second
        /// </summary>
        public int TimeOutTime = 1000;

        public void Connect(IPAddress address, int port)
        {
            listener = new TcpListener(address, port);
            listener.Start();

            StartAwaitClients();
        }

        public void Disconnect()
        {
            ShutDown = true;

            awaitConnections.Wait();
            awaitConnections.Dispose();

            foreach (var player in GameStore.Instance.Game.PlayerList)
            {
                player.Value.Wait();
                player.Value.Dispose();
                player.Key.Client.Close();
            }

            listener.Stop();
        }

        private async void AwaitClients()
        {
            while (!ShutDown)
            {

                var client = await listener.AcceptTcpClientAsync();
                //if (!client.Client.Poll(100, SelectMode.SelectRead)) continue;
                var task = Task.Factory.StartNew(() => ProcessClientStream(client));
                var player = new Player
                {
                    Client = client,
                    Id = GameStore.Instance.Game.PlayerList.Count,
                    PlayerAddress = (IPEndPoint) client.Client.RemoteEndPoint,
                    //Player Name Is Changed just after a client connected
                    PlayerName = null,
                    Port = NetworkOptions.Port
                };
                GameStore.Instance.Game.PlayerList.Add(player, task);
                ClientConnectedEvent?.Invoke((IPEndPoint) client.Client.RemoteEndPoint);
            }
        }

        public void StopAwaitClients()
        {
            awaitConnections.Wait();
            awaitConnections.Dispose();
        }

        public void StartAwaitClients()
        {
            if (awaitConnections == null || awaitConnections.IsCompleted || awaitConnections.IsCanceled) {
                awaitConnections = Task.Factory.StartNew(AwaitClients);
            }
            else {
                Console.WriteLine("Cannot start a running task");
            }
        }

        private async void ProcessClientStream(TcpClient client)
        {
            var stream = client.GetStream();

            while (!ShutDown)
            {
                if (client.Client != null && client.Connected)
                {
                    //if (client.Client.Poll(TimeOutTime, SelectMode.SelectRead))
                    //{
                    //    Player player = null;
                    //    byte[] buff = new byte[1];
                    //    if (client.Client.Receive(buff, SocketFlags.Peek) == 0)
                    //    {
                    //        Client Disconnected
                    //        foreach (var pl in GameStore.Instance.Game.PlayerList)
                    //        {
                    //            if (pl.Key.Client == client)
                    //            {
                    //                player = pl.Key;
                    //                Console.WriteLine("A player : " + pl.Key.PlayerAddress + " has disconnected");
                    //                pl.Value.Dispose();
                    //            }
                    //        }
                    //        if (player != null) GameStore.Instance.Game.PlayerList.Remove(player);
                    //        return;
                    //    }
                    //    else
                    //    {

                    //    }
                    //     = GameStore.Instance.Game.GetPlayerFromIpEndPoint((IPEndPoint)client.Client.RemoteEndPoint);
                    //    Console.WriteLine("A player : " + pl.PlayerAddress + " has disconnected");
                    //    pl.Value.Wait();
                    //}
                    var data = new byte[client.ReceiveBufferSize];
                    int count;
                    try
                    {
                        count = await stream.ReadAsync(data, 0, client.ReceiveBufferSize);
                    }
                    //A player Just Disconnected Sudden Disconnection
                    //The listener tried to read from a stream that was suddenly broked
                    catch (IOException)
                    {
                        DisconnectClient(client);
                        //Wait for a new client to connect
                        StartAwaitClients();
                        return;
                    }
                    //Client is Still Connected
                    var message = Encoding.ASCII.GetString(data, 0, count);
                    //Message Received just after client closes his stream
                    if (message == CodeMessages.PLAYER_DISCONNECTED)
                    {
                        //GameStore.Instance.Game.GetPlayer()
                        DisconnectClient(client);
                        //Wait for a new client to connect
                        StartAwaitClients();
                        return;
                    }
                    var address = (IPEndPoint)client.Client.RemoteEndPoint;
                    MessageReceivedEvent?.Invoke(new Message(message, address));
                }
            }
        }

        public Player GetPlayer(TcpClient client)
        {
            return GameStore.Instance.Game.GetPlayerByTcpClient(client);
        }

        public void DisconnectClient(TcpClient client)
        {
            Player p = null;
            foreach (var pl in GameStore.Instance.Game.PlayerList)
            {
                if (pl.Key.Client == client)
                {
                    p = pl.Key;
                    Console.WriteLine("A player : " + pl.Key.PlayerAddress + " has disconnected");
                    pl.Value.Wait();
                    pl.Value.Dispose();
                }
            }
            if (p != null) GameStore.Instance.Game.PlayerList.Remove(p);
        }

        public void Send(Player player, string message)
        {
            var buffer = new byte[message.Length * sizeof(char)];
            buffer = Encoding.ASCII.GetBytes(message);

            var client = player.Client;
            client.GetStream().Write(buffer, 0, buffer.Length);
        }

        public void Send(NetworkStream stream, string message)
        {
            var buffer = new byte[message.Length * sizeof(char)];
            buffer = Encoding.ASCII.GetBytes(message);

            stream.Write(buffer, 0, buffer.Length);
        }

        public void SendAll(string message)
        {
            foreach (var player in GameStore.Instance.Game.PlayerList)
            {
                Send(player.Key, message);
            }
        }
    }
}
