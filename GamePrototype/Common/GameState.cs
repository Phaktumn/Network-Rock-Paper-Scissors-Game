using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum GameModes
    {
        ConnectionOpen,
        ConnectionClosed,
        GameInitializing,
        GameStarted,
        RoundEnded,
        GameEnded
    }
}
