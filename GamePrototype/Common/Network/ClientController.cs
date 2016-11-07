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
        public bool ShutDown = false;

        private bool state = true;

        GameModes clientGameState = GameModes.ConnectionClosed;

        public ClientController()
        {
            client = new TcpClient();
        }

        public bool Connect(string address, int port)
        {
            try
            {
                client.Connect(address, port);
                stream = client.GetStream();
                BeginReceive();
                return state = true;
            }
            catch (SocketException)
            { 
                return state = false;
            }
        }

        public void Disconnect()
        {
            Send(CodeMessages.PLAYER_DISCONNECTED);
            ShutDown = false;
            ClientDisconnectedEvent?.Invoke(this.Client);

            if (receiveTask != null) {
                receiveTask.Wait();
                receiveTask.Dispose();
            }
            //Wait 1 sec to close the stream
            //So data will not be lost
            stream.Flush();
            stream.Close(timeout: 1000);
            client.Client.Close();
            client.Close();
        }

        public void Send(string message)
        {
            var bytes = Encoding.ASCII.GetBytes(message.ToCharArray());
            stream.Write(bytes, 0, bytes.Length);
        }

        private void BeginReceive()
        {
            receiveTask = Task.Factory.StartNew(async () =>
            {
                while (!ShutDown && Client.Connected)
                {
                    var data = new byte[client.ReceiveBufferSize];
                    var count = await stream.ReadAsync(data, 0, client.ReceiveBufferSize);
                    var message = Encoding.ASCII.GetString(data, 0, count);

                    var address = (IPEndPoint) client.Client.RemoteEndPoint;
                    MessageReceivedEvent?.Invoke(new Message(message, address), message.Split('|'));
                }
            });
        }
    }
}
