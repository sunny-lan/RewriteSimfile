namespace OsuSM
{
    class TimingPoint
    {
        /// <summary>
        /// Time in PPQ
        /// </summary>
        public long Time;
        /// <summary>
        /// Milliseconds per beat
        /// </summary>
        public double Tempo;
        /// <summary>
        /// Beats per measure
        /// </summary>
        public int Meter;

        /// <summary>
        /// milliseconds into the song
        /// </summary>
        public long SongTime;

    }
}
