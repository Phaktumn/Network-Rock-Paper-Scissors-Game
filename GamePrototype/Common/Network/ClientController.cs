using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Network
{
    public class ClientController
    {
        public delegate void OnMessageReceivedEventHandler(Message packet, string[] data);
        public delegate void OnClientDisconnectedEventHandler(TcpClient client);
        public event OnMessageReceivedEventHandler MessageReceivedEvent;
        public event OnClientDisconnectedEventHandler ClientDisconnectedEvent;

        public IPEndPoint EndPoint { get { return (IPEndPoint) client.Client.RemoteEndPoint; } }
        public TcpClient Client {  get { return client; } }

        private TcpClient client;
        private NetworkStream stream;
        private Task receiveTask;
        public bool shutDown = false;

        private bool state = true;

        GameModes clientGameState = GameModes.ConnectionClosed;

        public ClientController()
        { }

        public bool Connect(string address, int port)
        {
            try
            {
                client = new TcpClient(address, port);
                BeginReceive();
                stream = client.GetStream();
                return state = true;
            }
            catch (SocketException)
            {
                return state = false;
            }
        }

        public void Disconnect()
        {
            shutDown = false;
            ClientDisconnectedEvent?.Invoke(this.Client);

            if (receiveTask != null)
            {
                receiveTask.Wait();
                receiveTask.Dispose();
            }
        }

        public void Send(string message)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            stream.Write(bytes, 0, bytes.Length);
        }

        public void BeginReceive()
        {
            receiveTask = Task.Factory.StartNew(async () =>
            {
                while (!shutDown &&  Client.Connected)
                {
                    byte[] data = new byte[client.ReceiveBufferSize];
                    int count = await stream.ReadAsync(data, 0, client.ReceiveBufferSize);
                    string message = Encoding.ASCII.GetString(data, 0, count);

                    IPEndPoint address = (IPEndPoint) client.Client.RemoteEndPoint;
                    MessageReceivedEvent?.Invoke(new Message(message, address), message.Split('|'));
                }
            });
        }
    }
}
