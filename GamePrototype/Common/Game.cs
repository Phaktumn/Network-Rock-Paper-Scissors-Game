using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common
{
    public class Game
    {
        public delegate void OnGameStateChangeEventHandler(GameModes mode);
        public event OnGameStateChangeEventHandler GameStateChangeEvent;

        public Dictionary<Player, Task> PlayerList { get; set; }
        public int TargetNumber { get; set; }
        public List<Move> MoveList { get; set; }

        public void OnGameStateChangeEvent(GameModes mode)
        {
            GameStateChangeEvent?.Invoke(mode);
        }
    }
}
