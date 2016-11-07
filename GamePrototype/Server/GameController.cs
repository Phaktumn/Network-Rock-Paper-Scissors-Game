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

        ///// <summary>
        ///// Always Make shure this var is correct or the game will not work as supposed
        ///// This is the max number of habilities so the server knows what to do!
        ///// </summary>
        //public readonly int NumberOfAbilities = 5;

        private static GameController _instance;
        private readonly List<Abilities> stack;

        private GameController()
        {
            stack = new List<Abilities>();
        }

        public void AddAttack(Abilities attack)
        {
            stack.Add(attack);
        }

        public void ClearStack()
        {
            stack.Clear();
            //battleAuxRes.Clear();
        }

        public BattleState Battle()
        {
            if (stack.Count != 2) return BattleState.None;
            var state = WhoWins(stack[0], stack[1]);
            stack.Clear();
            return state;
        }

        //public List<int> battleAuxRes = new List<int>();

        private BattleState WhoWins(Abilities abilitie1, Abilities abilitie2)
        {
            var ab1 = (int)abilitie1;
            var ab2 = (int)abilitie2;

            //if (ab1 == ab2)
            //{
            //    return BattleState.Draw;
            //}

            //for (int i = 1; i <= NumberOfAbilities - 2; i++)
            //{
            //    var res1 = ab1 + i;
            //    if (res1 > NumberOfAbilities) {
            //        res1 -= 5;
            //    }
            //    battleAuxRes.Add(res1);
            //}
            //var res2 = ab1 - 1;
            //if (res2 <= 0) res2 += 5;
            //battleAuxRes.Add(res2);

            //foreach (var res in battleAuxRes)
            //{
            //    if (ab2 != res) continue;
            //    if (IsOdd(res))  return BattleState.Lost; 
            //    if (IsEven(res)) return BattleState.Won;
            //}

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
            //return BattleState.None;
        }

        //private bool IsOdd(int value)
        //{
        //    return value % 2 != 0;
        //}

        //private bool IsEven(int value)
        //{
        //    return value % 2 == 0;
        //}
    }
}
