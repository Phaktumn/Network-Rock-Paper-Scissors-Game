using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Network;
using Microsoft.Win32;
using Server.Controlls;

namespace Server
{
    public class ServerController
    {
        private GameModes _serverState;
        private TcpObserver tcpListener;

        private bool shutdown;

        private int readyCounter = 0;

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
            Colorful.Console.WriteAscii("RPSLS GAME", Color.CadetBlue);
            for (;;)
            {
                switch (_serverState)
                {
                    case GameModes.ConnectionClosed:

                        //GameStore.Instance.Game.OnGameStateChangeEvent(GameModes.ConnectionClosed);

                        tcpListener = new TcpObserver();
                        tcpListener.Connect(IPAddress.Parse(NetworkOptions.Ip), NetworkOptions.Port);
                        tcpListener.MessageReceivedEvent += TcpListener_MessageReceivedEvent;
                        tcpListener.ClientConnectedEvent += TcpListenerOnClientConnectedEvent;
                        Write("Server EndPoint : " + tcpListener.Listener.LocalEndpoint.ToString(), Color.Aqua);
                        //tcpListener.Listener.Server.Blocking = false;
                        Write("Server Blocking Connections : " + tcpListener.Listener.Server.Blocking, Color.Aqua);

                        GameStore.Instance.Game.OnGameStateChangeEvent(GameModes.ConnectionOpen);

                        break;
                    case GameModes.ConnectionOpen:
                        Write("Not Enougth Players", Color.Red);
                        if (GameStore.Instance.Game.PlayerList.Count >= 2)
                        {
                            Write("Session in Full", Color.LawnGreen);
                            Write("Starting Session", Color.LawnGreen);
                            tcpListener.StopAwaitClients();
                            GameStore.Instance.Game.OnGameStateChangeEvent(GameModes.GameInitializing);
                            tcpListener.SendAll(CodeMessages.GAME_INITIALIZING.Message);
                        }
                        break;
                    case GameModes.GameInitializing:

                        GameStore.Instance.Game.OnGameStateChangeEvent(GameModes.GameStarted);
                        tcpListener.SendAll(CodeMessages.GAME_STARTING.Message);
                        break;
                    case GameModes.GameStarted:
                        if (GameStore.Instance.Game.PlayerList.Count < 2)
                        {
                            GameStore.Instance.Game.OnGameStateChangeEvent(GameModes.ConnectionOpen);
                            tcpListener.SendAll(CodeMessages.GAME_MAIN_MENU.Message);
                            //tcpListener.SendAll(CodeMessages.PUBLIC_SERVER_MESSAGE.Message + " A Player Just Disconnected going back to menu");
                        }
                        break;
                    case GameModes.GameEnded:

                        break;
                    case GameModes.RoundEnded:
                        if (GameStore.Instance.Game.PlayerList.Count < 2)
                        {
                            GameStore.Instance.Game.OnGameStateChangeEvent(GameModes.ConnectionOpen);
                            tcpListener.SendAll(CodeMessages.GAME_MAIN_MENU.Message);
                            //tcpListener.SendAll(CodeMessages.PUBLIC_SERVER_MESSAGE.Message + "A Player Just Disconnected going back to menu");
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                Thread.Sleep(1000);
            }
        }
        private void TcpListenerOnClientConnectedEvent(IPEndPoint address)
        {
            Write("Connected Address : " + address, Color.CornflowerBlue);
        }

        /// <summary>
        /// When A Message is Received from a client
        /// </summary>
        /// <param name="packet"></param>
        private void TcpListener_MessageReceivedEvent(Message packet)
        {
            string[] rawMessage = Message.Deserialize(packet);
            string receivedMessage = rawMessage[0];
            Player player = GameStore.Instance.Game.GetPlayerFromIpEndPoint(packet.Sender);
            Write("Received from " + player.PlayerAddress + " -> " + packet.Data, Color.OrangeRed);
            switch (_serverState)
            {
                case GameModes.ConnectionOpen:
                    //Is the client already connected?
                    //The client already has a name dont change
                    if (player.PlayerName != null) {
                        break;
                    }
                    Write("Players : " + GameStore.Instance.Game.PlayerList.Count, Color.Blue);
                    player.PlayerName = receivedMessage;
                    break;
                case GameModes.ConnectionClosed:
                    break;
                case GameModes.GameInitializing:
                    break;
                case GameModes.GameStarted:
                    //Message Received as a ability
                    int used = int.Parse(receivedMessage);

                    Write("||" + player.PlayerName + "|| Used: " + (Abilities) used, Color.Red);
                    GameStore.Instance.Game.AttackOrder.Add(player);
                    GameController.Instance.addAttack((Abilities) used);

                    tcpListener.Send(player, 
                        $"{CodeMessages.PUBLIC_SERVER_MESSAGE.Message} You are now waiting for other Players to play {CodeMessages.PLAYER_WAITING}");
                      
                    if (GameStore.Instance.Game.AttackOrder.Count == GameStore.Instance.Game.PlayerList.Count)
                    {
                        Write("Round Ended", Color.Red);
                        //All Players Are Ready!
                        //Show Who Won and Who Lost
                        var state = GameController.Instance.Battle();

                        if (state != BattleState.None)
                        {
                            Write("Round Finished", Color.Red);
                            var p1 = GameStore.Instance.Game.AttackOrder[0];
                            var p2 = GameStore.Instance.Game.AttackOrder[1];
                            if (state == BattleState.Lost)
                            {
                                Write(p1.PlayerName + " Lost", Color.Red);
                                Write(p2.PlayerName + " Won", Color.Green);
                                tcpListener.Send(p1, $"{CodeMessages.PUBLIC_SERVER_MESSAGE.Message} You have : {state}");
                                tcpListener.Send(p2,
                                    $"{CodeMessages.PUBLIC_SERVER_MESSAGE.Message} You have : {BattleState.Won}");
                            }
                            if (state == BattleState.Won)
                            {
                                Write(p1.PlayerName + " Won", Color.Green);
                                Write(p2.PlayerName + " Lost", Color.Red);
                                tcpListener.Send(p1, $"{CodeMessages.PUBLIC_SERVER_MESSAGE.Message} You have : {state}");
                                tcpListener.Send(p2,
                                    $"{CodeMessages.PUBLIC_SERVER_MESSAGE.Message} You have : {BattleState.Lost}");
                            }
                            if (state == BattleState.Draw)
                            {
                                Write(p1.PlayerName + " Draw", Color.Red);
                                Write(p2.PlayerName + " Draw", Color.Green);
                                tcpListener.Send(p1, $"{CodeMessages.PUBLIC_SERVER_MESSAGE.Message} You have : {state}");
                                tcpListener.Send(p2, $"{CodeMessages.PUBLIC_SERVER_MESSAGE.Message} You have : {state}");
                            }
                        }
                        _serverState = GameModes.RoundEnded;
                        tcpListener.SendAll(CodeMessages.GAME_ROUND_ENDED.Message);
                    }
                    break;
                case GameModes.RoundEnded:
                    //A player replied with success
                    readyCounter++;
                    if (readyCounter == 2)
                    {
                        //Get Stuff Ready 
                        Write("Players can now Play", Color.GreenYellow);
                        //NOT USING THIS AT THE MOMENT
                        //tcpListener.SendAll($"{CodeMessages.INTERNAL_SERVER_MESSAGE.Message}canplay");
                        GameStore.Instance.Game.AttackOrder.Clear();
                        readyCounter = 0;
                        GameStore.Instance.Game.OnGameStateChangeEvent(GameModes.GameStarted);
                        tcpListener.SendAll(CodeMessages.GAME_STARTING.Message);
                    }
                    break;
                case GameModes.GameEnded:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void GameOnGameStateChangeEvent(GameModes mode)
        {
            _serverState = mode;
            Write(mode.ToString(), Color.Aqua);
        }

        private static void Write(string text, Color col)
        {
            Colorful.Console.WriteFormatted(DateTime.Now.ToString(CultureInfo.CurrentCulture) + " -> ", Color.DimGray);
            Console.Write(text + "\n", col);
        }
    }
}
