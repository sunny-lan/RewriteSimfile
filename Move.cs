namespace OsuSM.alg1
{

    


    struct Move
    {

        /// <summary>
        /// -1 if no move
        /// </summary>
        public int L;
        /// <summary>
        /// -1 if no move
        /// </summary>
        public int R;

        public Move(int foot, int arrow)
        {
            L = R = -1;
            Step(foot, arrow);
        }

        public void Reset()
        {
            L = R = -1;
        }
        public const int LEFT = 0, RIGHT = 1;
        public void Step(int foot, int arrow)
        {
            if (foot == LEFT)
                L = arrow;
            else
                R = arrow;
        }
        public int Pos(int foot)
        {
            return foot == LEFT ? L : R;
        }

        public bool Tap(int foot)
        {
            return Pos(foot) != -1;
        }
    }
}
