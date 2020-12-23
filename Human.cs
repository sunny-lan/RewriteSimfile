
using System;
using System.Collections.Generic;
using System.Text;

namespace OsuSM.alg3
{
    static class Extensions
    {
        /// <summary>
        /// Transforms an arrow relative to the direction we are facing
        /// </summary>
        /// <param name="arrow"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        public static Arrow Rotate(this Arrow arrow, Arrow root)
        {
            return (Arrow)(((int)arrow - (int)root + 4) % 4);
        }

        /// <summary>
        /// returns the actual index on the ddr screen
        /// </summary>
        /// <param name="arrow"></param>
        /// <returns></returns>
        public static int Index(this Arrow arrow)
        {
            switch (arrow)
            {
                case Arrow.LEFT: return 0;
                case Arrow.UP: return 1;
                case Arrow.DOWN: return 2;
                case Arrow.RIGHT: return 3;
            }
            return -1;
        }
    }
    enum Arrow
    {
        UP = 0, RIGHT = 1, DOWN = 2, LEFT = 3
    }
    enum Move
    {
        NONE = 0, LEFT = 1, RIGHT = 2, JUMP = 3
    }
    struct FootState
    {
        /// <summary>
        /// Position of the foot
        /// Value from Consts.Arrow
        /// </summary>
        public Arrow L, R;
        /// <summary>
        /// Represents the direction the person is facing
        /// </summary>
        public Arrow Direction;
    }

    class Human
    {
        public static List<FootState> FootStates;
        public static Dictionary<FootState, int> FootstateID;
        static Human()
        {
            FootStates = new List<FootState>();
            FootstateID = new Dictionary<FootState, int>();
            foreach (Arrow dir in Enum.GetValues(typeof(Arrow)))
            {
                foreach (Arrow l in Enum.GetValues(typeof(Arrow)))
                {
                    if (l.Rotate(dir) == Arrow.RIGHT) continue;
                    foreach (Arrow r in Enum.GetValues(typeof(Arrow)))
                    {
                        if (r.Rotate(dir) == Arrow.RIGHT) continue;
                        var f = new FootState
                        {
                            L = l,
                            R = r,
                            Direction = dir,
                        };
                        FootstateID[f] = FootStates.Count;
                        FootStates.Add(f);
                    }
                }
            }
        }

    }
}
