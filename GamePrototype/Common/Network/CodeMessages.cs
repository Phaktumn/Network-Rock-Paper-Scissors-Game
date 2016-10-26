﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Common.Network
{
    public static class CodeMessages
    {
        public struct NetworkCode
        {
            public NetworkCode(string message, Color messageColor, bool inter) : this()
            {
                Message = message.Split(MESSAGE_SPLITER_CODE.ToCharArray())[0];
                MessageColor = messageColor;
                isInternal = inter;
            }

            public string Message;

            public Color MessageColor;
            public bool isInternal;
        }

        private static string SERVER_MESSAGE_CODE = "(Server Message)";
        public static string MESSAGE_SPLITER_CODE = "|";
        private static string GAME_INITIALIZING_CODE = "Initializing";
        private static string GAME_STARTING_CODE = "Starting";
        private static string GAME_ENDED_CODE = "Ended";
        private static string GAME_ROUND_ENDED_CODE = "REnded";

        /// <summary>
        /// Player State
        /// </summary>
        public static string PLAYER_WAITING = "waiting";
        public static string PLAYER_DISCONNECTED = "disconnect";

        public static NetworkCode PUBLIC_SERVER_MESSAGE = new NetworkCode($"{SERVER_MESSAGE_CODE}{MESSAGE_SPLITER_CODE}", Color.Aqua, true);
        public static NetworkCode INTERNAL_SERVER_MESSAGE = new NetworkCode($"{SERVER_MESSAGE_CODE}", Color.BlueViolet, false);

        public static NetworkCode GAME_INITIALIZING = new NetworkCode($"{GAME_INITIALIZING_CODE}{MESSAGE_SPLITER_CODE}", Color.Aqua, false);
        public static NetworkCode GAME_STARTING = new NetworkCode($"{GAME_STARTING_CODE}{MESSAGE_SPLITER_CODE}", Color.Aqua, false);
        public static NetworkCode GAME_ENDED = new NetworkCode($"{GAME_ENDED_CODE}{MESSAGE_SPLITER_CODE}", Color.Aqua, false);
        public static NetworkCode GAME_ROUND_ENDED = new NetworkCode($"{GAME_ROUND_ENDED_CODE}{MESSAGE_SPLITER_CODE}", Color.Aqua, false);

        public static bool IsInternal(NetworkCode code)
        {
            return code.isInternal;
        }
    }
}
