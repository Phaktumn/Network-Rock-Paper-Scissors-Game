using System;
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

        public void Connect(IPAddress address, int port)
        {
            listener = new TcpListener(address, port);
            listener.Start();

            awaitConnections = Task.Factory.StartNew(AwaitClients);
        }

        public void Disconnect()
        {
            ShutDown = true;

            awaitConnections.Wait();
            awaitConnections.Dispose();

            foreach (KeyValuePair<Player, Task> player in GameStore.Instance.Game.PlayerList)
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
                var task = Task.Factory.StartNew(() => ProcessClientStream(client));
                var player = new Player
                {
                    Client = client,
                    Id = GameStore.Instance.Game.PlayerList.Count,
                    PlayerAddress = (IPEndPoint) client.Client.RemoteEndPoint,
                    PlayerName = null,
                    Port = 7777
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

        private async void ProcessClientStream(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            while (client.Connected)
            {
                int count;
                byte[] data = new byte[client.ReceiveBufferSize];
                try
                {
                    count = await stream.ReadAsync(data, 0, client.ReceiveBufferSize);
                }
                //A player Just Disconnected
                catch (IOException)
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
                    GameStore.Instance.Game.PlayerList.Remove(p);
                    return;
                }

                string message = Encoding.ASCII.GetString(data, 0, count);
                IPEndPoint address = (IPEndPoint) client.Client.RemoteEndPoint;
                MessageReceivedEvent?.Invoke(new Message(message, address));
            }
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
            byte[] buffer = new byte[message.Length * sizeof(char)];
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
