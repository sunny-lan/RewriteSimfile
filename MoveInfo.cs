using System;
using System.Collections.Generic;
using System.Text;

namespace OsuSM
{
    namespace alg2
    {
        struct FootState
        {
            public const int LEFT = 0, DOWN = 1, UP = 2, RIGHT = 3;
            public int L, R;
            public const int REST = 0, TAP = 1, HOLD = 2;
            public int LMode, RMode;
            public int Facing;//0 for clockwise, 1 for counterclockwise
            public int Weight;//0 for L, 1 for R

        }


        class MoveInfo
        {
            public bool Crossover;
            public bool FootSwap;
            public bool DoubleTap;
            public bool HalfSpin;
            public bool Step;
            public bool Jump;
        }

        static class Moves
        {
            public static Dictionary<FootState, int> Code = new Dictionary<FootState, int>();
            public static List<FootState> FootStates = new List<FootState>();
            public static MoveInfo[,] Info;

            static void GenFs()
            {
                for (int l = 0; l < 4; l++)
                    for (int r = 0; r < 4; r++)
                        for (int lm = 0; lm < 3; lm++)
                            for (int rm = 0; rm < 3; rm++)
                                for (int f = 0; f < 2; f++)
                                    for (int w = 0; w < 2; w++)
                                    {
                                        var fs = new FootState
                                        {
                                            Facing = f,
                                            L = l,
                                            R = r,
                                            LMode = lm,
                                            RMode = rm,
                                            Weight = w
                                        };
                                        Code[fs] = FootStates.Count;
                                        FootStates.Add(fs);
                                    }
            }



            static Moves()
            {
                GenFs();
                Info = new MoveInfo[Code.Count, Code.Count];
                for (int i = 0; i < Code.Count; i++)
                {
                    var src = FootStates[i];
                    for (int j = 0; j < Code.Count; j++)
                    {
                        var dst = FootStates[j];

                        MoveInfo m = new MoveInfo { };

                        if (dst.L == FootState.RIGHT)
                        {

                        }

                        Info[i, j] = m;
                    }
                }
            }
        }
    }
}
