using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using Common;

namespace Server.Controlls
{
    public class GameController
    {
        public static GameController Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = new GameController();
                return _instance;
            }
        }

        private static GameController _instance;
        private readonly List<Abilities> stack;

        public GameController()
        {
            stack = new List<Abilities>();
        }

        public void addAttack(Abilities attack)
        {
            stack.Add(attack);
        }

        public BattleState Battle()
        {
            if (stack.Count != 2) return BattleState.None;
            var state = WhoWins(stack[0], stack[1]);
            stack.Clear();
            return state;
        }

        private static BattleState WhoWins(Abilities abilitie1, Abilities abilitie2)
        {
            var ab1 = (int)abilitie1;
            var ab2 = (int)abilitie2;

            var res1 = ab1 + 2; //lose
            var res2 = ab1 + 1; //win
            var res3 = ab1 - 1; //Lose
            var res4 = ab1 + 3; //Win

            if (res1 > 5) res1 -= 5;
            if (res2 > 5) res2 -= 5;
            if (res3 < 0) res3 += 5;
            if (res4 > 5) res4 -= 5;

            if (ab1 == ab2) return BattleState.Draw;
            if (res1 == ab2) return BattleState.Lost;
            if (res2 == ab2) return BattleState.Won;
            if (res3 == ab2) return BattleState.Lost;
            return res4 == ab2 ? BattleState.Won : BattleState.None;
        }
    }
}
