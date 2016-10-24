using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common
{
    public class Game
    {
        public Dictionary<Player, Task> PlayerList { get; set; }
        public int TargetNumber { get; set; }
        public List<Move> MoveList { get; set; }
        public GameModes GameState { get; set; }
    }
}
