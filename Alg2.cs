using System;
using System.Collections.Generic;
using System.Text;

namespace OsuSM.alg2
{

    class Algorithm
    {
        public BasicSongInfo Info;

        /// <summary>
        /// If both feet are allowed to be on the same arrow
        /// TODO: enable this
        /// </summary>
        private bool FootSwitch = false;

        /// <summary>
        /// If crossovers are allowed
        /// </summary>
        private bool Crossover = true;


        /// <summary>
        /// If halfspin is allowed
        /// TODO
        /// </summary>
        private bool HalfSpin = false;


        /// <summary>
        /// If backwards is allowed. TODO enable this
        /// </summary>
        private bool Backwards = false;


        public void Run()
        {
            f(0, 0);
        }

        public double StayScore = -1;

        /// <summary>
        /// allow foot to tap the same square twice
        /// </summary>
        public bool DoubleTap = true;

        double f(int note, int footState)
        {
            if (note == Info.NoteTimes.Count)
                return 0;
            return 1;

        }
    }
}
