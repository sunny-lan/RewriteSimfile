using System.Collections.Generic;

namespace OsuSM
{
    class BasicSongInfo
    {
        /// <summary>
        /// Offset,in ms of the first event to the beginning of audio
        /// </summary>
        public long AudioOffset;

        public long PPQ;
        public List<long> NoteTimes;
        public List<TimingPoint> TimingPoints;

    }
}
