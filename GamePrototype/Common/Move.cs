using System;
using Common;

namespace Common
{
    public abstract class Move
    {
        public int Id { get; set; }
        public Player TargetPlayer { get; set; }
        public DateTime MoveDateTime { get; set; }
        public Abilities PlayedAbility { get; set; }
    }
}
