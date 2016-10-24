using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Network;

namespace Server
{
    public class ServerController
    {
        private GameModes _serverState;
        private TcpObserver tcpListener;

        private bool shutdown;

        public ServerController()
        {
            _serverState = GameModes.ConnectionClosed;
            GameStore.Instance.Game.PlayerList = new Dictionary<Player, Task>();
            GameStore.Instance.Game.GameStateChangeEvent += GameOnGameStateChangeEvent;
        }

        private void Shutdown(Message packet, string[] msg)
        {
            tcpListener.Disconnect();
            shutdown = true;
        }

        public void StartServer()
        {
            //Task.Factory.StartNew(async () => 
            //{
                for (;;)
                {
                    switch (_serverState)
                    {
                        case GameModes.ConnectionClosed:

                            GameStore.Instance.Game.OnGameStateChangeEvent(GameModes.ConnectionClosed);

                            tcpListener = new TcpObserver();
                            tcpListener.Connect(IPAddress.Parse("127.0.0.1"), 5000);
                            
                            GameStore.Instance.Game.OnGameStateChangeEvent(GameModes.ConnectionOpen);
                            
                            break;
                        case GameModes.ConnectionOpen:

                            //Write(_serverState.ToString(), Color.Aqua);
                            
                            if (GameStore.Instance.Game.PlayerList.Count >= 2) {
                                tcpListener.StopAwaitClients();
                                GameStore.Instance.Game.OnGameStateChangeEvent(GameModes.GameInitializing);
                            }

                            //if (tcpListener.Listener.Pending())
                            //{

                                //TcpClient tcpClient = _tcpListener.AcceptTcpClient();
                                //await ReadMessage(tcpClient);
                                //if (tcpClient.Connected)
                                //{
                                    //WriteMessage(tcpClient, "Bem vindo");
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
                                //}
                                //if (GameStore.Instance.Game.PlayerList.Count > 1)
                                //{
                                    //_serverState = GameModes.GameInitializing;
                                //}
                            //}
                            break;
                        case GameModes.GameInitializing:
                            //Console.WriteLine(_serverState.ToString()); 
                            // TODO: Initialize Game Logic
                            GameStore.Instance.Game.OnGameStateChangeEvent(GameModes.GameStarted);
                        break;
                        case GameModes.GameStarted:
                            //Console.WriteLine(_serverState.ToString());
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
                            GameStore.Instance.Game.OnGameStateChangeEvent(GameModes.GameEnded);
                            break;
                        case GameModes.GameEnded:
                            Console.WriteLine(_serverState.ToString());
                            Console.WriteLine("Game ended...");
                            break;
                    }
                }
            //});
        }

        private void GameOnGameStateChangeEvent(GameModes mode)
        {
            _serverState = mode;
           Write(mode.ToString(), Color.Aqua);
        }

        private static void Write(string text, Color col)
        {
            Colorful.Console.WriteFormatted(DateTime.Now.ToString(CultureInfo.CurrentCulture)
                + " -> ", Color.DimGray);
            Console.Write(text + "\n", col);
        }
    }
}
