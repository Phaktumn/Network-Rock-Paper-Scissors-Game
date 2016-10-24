using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Server
{
    public class ServerController
    {
        private GameModes _serverState;
        private TcpListener _tcpListener;

        public ServerController()
        {
            _serverState = GameModes.ConnectionClosed;
            GameStore.Instance.Game.PlayerList = new List<Player>();
        }

        public void StartServer()
        {
            Task.Factory.StartNew(async () => { 
            for (;;) { 
                switch (_serverState)
                {
                    case GameModes.ConnectionClosed:
                        Console.WriteLine(_serverState.ToString());
                        _tcpListener = new TcpListener(IPAddress.Any, 7777);
                        _tcpListener.Start();
                        _serverState = GameModes.ConnectionOpen;
                    break;
                    case GameModes.ConnectionOpen:
                        Console.WriteLine(_serverState.ToString());
                        if (_tcpListener.Pending())
                        {
                            TcpClient tcpClient = _tcpListener.AcceptTcpClient();
                            await ReadMessage(tcpClient);
                            if (tcpClient.Connected)
                            {
                                WriteMessage(tcpClient, "Bem vindo");
                                //WriteMessage(tcpClient, "Introduza o seu nome");
                                //string nome = ReadMessage(tcpClient);
                                //Console.WriteLine(nome);

                                //Player player = new Player()
                                //{
                                //    Client =  tcpClient,
                                //    Id = GameStore.Instance.Game.PlayerList.Count,
                                //    PlayerAddress = tcpClient.Client.RemoteEndPoint,
                                //    PlayerName = 
                                //}

                                //GameStore.Instance.Game.PlayerList.Add(tcpClient);
                            }
                            if (GameStore.Instance.Game.PlayerList.Count > 1)
                            {
                                _serverState = GameModes.GameInitializing;
                            }
                        }
                    break;
                    case GameModes.GameInitializing:
                        Console.WriteLine(_serverState.ToString());
                        // TODO: Initialize Game Logic
                        _serverState = GameModes.GameStarted;
                        break;
                    case GameModes.GameStarted:
                        Console.WriteLine(_serverState.ToString());
                        //for (int x = 0; x < _tcpClientList.Count; x++)
                        //{
                        //    WriteMessage(_tcpClientList[x], "Joga...");
                        //    string message = ReadMessage(_tcpClientList[x]);
                        //    foreach (TcpClient tcpClient in _tcpClientList)
                        //    {
                        //        WriteMessage(tcpClient, "Foi jogado..."+ message);
                        //    }
                        //    if (message == "Ganhei")
                        //    {
                        //        _serverState = GameModes.GameEnded;
                        //        break;
                        //    }
                        //    if (x >= _tcpClientList.Count)
                        //    {
                        //        x = 0;
                        //    }
                        //}
                        break;
                    case GameModes.GameEnded:
                        Console.WriteLine(_serverState.ToString());
                        Console.WriteLine("Game ended...");
                        break;
                }
            }
            });
        }

        private void WriteMessage(TcpClient tcpClient, string message)
        {
            BinaryWriter binaryWriter = new BinaryWriter(tcpClient.GetStream());
            binaryWriter.Write(message);
        }

        byte[] buffer = new byte[1024];
        MemoryStream ms = new MemoryStream();

        //public async Task<Received> ReadMessage(TcpClient tcpClient)
        //{
        //    var bytesRead = await tcpClient.GetStream().ReadAsync(buffer, 0, buffer.Length);
        //    if (bytesRead > 0) {
                
        //    }
        //}
    }
}
