using System;
using System.Data.Odbc;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Client
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
        private bool shutDown;

        private bool KeepTrying = true;

        GameModes clientGameState = GameModes.ConnectionClosed;

        public ClientController()
        { }

        public void Connect(string address, int port)
        {
            while (KeepTrying)
            {
                try
                {
                    client = new TcpClient(address, port);
                    KeepTrying = false;
                }
                catch (SocketException)
                {
                    Colorful.Console.WriteLine("No Server Found");
                    KeepTrying = true;
                }
            }
            stream = client.GetStream();
            Colorful.Console.WriteLine("Connected");
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

        public void StartClient()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    
                }
            }); 
        }

        public void UpdateClient()
        {
            while (true)
            {
                switch (clientGameState)
                {
                    case GameModes.ConnectionOpen:
                        break;
                    case GameModes.ConnectionClosed:
                        break;
                    case GameModes.GameInitializing:
                        break;
                    case GameModes.GameStarted:
                        break;
                    case GameModes.RoundEnded:
                        break;
                    case GameModes.GameEnded:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
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
                while (!shutDown)
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
