using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
        public sealed class GameStore
        {
            private static GameStore _instance;
            public Game Game { get; set; }

            private GameStore()
            {
                Game = new Game();
            }

            public static GameStore Instance => _instance ?? (_instance = new GameStore());
        }
}
