using System;
using System.Text;

namespace OsuSM.alg1
{


    struct FootState
    {
        public const int LEFT = 0, RIGHT = 1;
        /// <summary>
        /// max feet 
        /// </summary>
        public const int MaxF = 4;

        /// <summary>
        /// max tired
        /// </summary>
        public const int MaxT = 8;

        /// <summary>
        /// left pos
        /// </summary>
        public int L;
        /// <summary>
        /// right pos
        /// </summary>
        public int R;
        /// <summary>
        /// left tiredness/cooldown 
        /// measured out of 8
        /// foot can only tap again once it reaches 0
        /// </summary>
        public int TL;
        /// <summary>
        /// right tiredness
        /// </summary>
        public int TR;


        public int Pos(int foot)
        {
            return foot == LEFT ? L : R;
        }

        public int Tired(int foot)
        {
            return foot == LEFT ? TL : TR;
        }

        public void Step(int foot, int arrow)
        {
            if (foot == LEFT)
                L = arrow;
            else
                R = arrow;
        }

        public void SetTired(int foot, int tiredness)
        {
            if (foot == LEFT)
                TL = Math.Clamp(tiredness, 0, MaxF - 1);
            else
                TR = Math.Clamp(tiredness, 0, MaxF - 1);
        }
    }

    class StepScoreGenerator
    {
        public Random Rng;

        /// <summary>
        /// Gives the scores per priority
        /// </summary>
        public int[] Scores = { 1, 10, 100, 1000 };

        public double[,] RandomStepScore(BasicSongInfo song)
        {
            var score = new double[song.NoteTimes.Count, 4];
            RandomizeStepScore(score, 0, song.NoteTimes.Count);
            return score;
        }

        public void RandomizeStepScore(double[,] score, int start, int end)
        {
            int[] importance = { 0, 1, 2, 3 };
            for (int i = start; i < end; i++)
            {
                for (int j = 0; j < 4; j++)
                    score[i, j] = Scores[importance[j]];
            }
        }
    }

    class Algorithm
    {
        public BasicSongInfo Info;
        public FootState[] Footstates;

        public Algorithm()
        {
            int F = FootState.MaxF, T = FootState.MaxT;
            Footstates = new FootState[F * F * T * T];
            for (int l = 0; l < F; l++)
            {
                for (int r = 0; r < F; r++)
                {
                    for (int tl = 0; tl < T; tl++)
                    {
                        for (int tr = 0; tr < T; tr++)
                        {
                            var f = new FootState
                            {
                                L = l,
                                R = r,
                                TL = tl,
                                TR = tr,
                            };
                            Footstates[id(f)] = f;
                        }
                    }
                }
            }
        }

        int id(in FootState s)
        {
            return s.L | s.R << 2 | s.TL << 4 | s.TR << 7;
        }

        /// <summary>
        /// the score from staying on note i to i+1
        /// should be negative
        /// </summary>
        double stayScore(int i)
        {
            return -Info.NoteTimes.Count / 2.0;
        }

        /// <summary>
        /// amount of time it takes for a foot to recover 1 tiredness
        /// should be eigth of the minimum time you are able to step a single foot in
        /// </summary>
        public long RecoveryInterval;

        long recoveryInterval(int i)
        {
            return RecoveryInterval;
        }

        /// <summary>
        /// DP[i,j] represents the best score to play from the ith note to the end of the song
        /// given that the player was in state J on the previous note
        /// </summary>
        double[,] dp;


        /// <summary>
        ///next[i,j] represents the best state for the player to move on the ith note
        ///given that the player was in state J on the previous note
        /// </summary>
        int[,] next;

        /// <summary>
        ///move[i,j] represents the best move for the player to make on the ith note
        ///given that the player was in state J on the previous note
        /// </summary>
        Move[,] move;

        /// <summary>
        /// The bonus to step on the ith note, jth arrow
        /// </summary>
        public double[,] StepScore;
        double stepScore(int i, int arrow)
        {
            return StepScore[i, arrow];
        }

        /// <summary>
        /// Allow two feet on one arrow?
        /// </summary>
        public bool AllowFootswap;

        public Move[] Run()
        {

            int N = Info.NoteTimes.Count;
            int F = Footstates.Length;

            dp = new double[N + 1, F];
            next = new int[N, F];

            move = new Move[N, F];

            for (int i = N - 1; i >= 0; i--)
            {
                ///duration between current and next note
                long t = long.MaxValue;
                if (i < N - 1)
                    t = Info.NoteTimes[i + 1] - Info.NoteTimes[i];


                long interval = recoveryInterval(i);
                int recovery = (int)Math.Min(FootState.MaxT, (t + interval - 1) / interval);

                for (int j = 0; j < F; j++)
                {
                    //if the player was in state J in the previous move
                    //they will now be in state recover(J, interval)
                    FootState cur = Footstates[j];
                    for (int foot = 0; foot < 2; foot++)
                        cur.SetTired(foot, cur.Tired(foot) - recovery);

                    //stay
                    Move altMove = new Move();
                    FootState altState = cur;
                    int best = id(altState);
                    //the score decays when staying, in order to make sure the algorithm places
                    //as many notes as possible
                    double mx = stayScore(i) + dp[i + 1, best];
                    altMove.Reset();
                    move[i, j] = altMove;

                    //step

                    for (int foot = 0; foot < 2; foot++)
                    {
                        //foot is too tired to step
                        if (cur.Tired(foot) != 0) continue;

                        for (int arrow = 0; arrow < 4; arrow++)
                        {
                            //if other foot is on arrow, don't
                            if (cur.Pos(1 - foot) == arrow) continue;

                            altState = cur;

                            altState.SetTired(foot, 7);
                            altState.Step(foot, arrow);

                            int altBest = id(altState);

                            altMove.Reset();
                            altMove.Step(foot, arrow);

                            double altMx = dp[i, altBest] + stepScore(i, arrow);
                            if (altMx > mx)
                            {
                                mx = altMx;
                                best = altBest;
                                move[i, j] = altMove;
                            }
                        }
                    }


                    //TODO jump

                    dp[i, j] = mx;
                    next[i, j] = best;
                }
                //TODO apply state scores

            }

            int b1 = -1;
            double mx1 = double.NegativeInfinity;
            for (int i = 0; i < F; i++)
            {
                var f = Footstates[i];
                if (f.TL == 0 && f.TR == 0)//tiredness at begin should be 0
                    if (dp[0, i] > mx1)
                    {
                        mx1 = dp[0, i];
                        b1 = i;
                    }
            }


            Move[] result = new Move[N];
            for (int i = 0; i < N; i++)
            {
                result[i] = move[i, b1];
                b1 = next[i, b1];
            }
            return result;
        }
    }
}
