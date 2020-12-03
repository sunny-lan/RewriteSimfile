
using OsuParsers.Beatmaps;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OsuSM
{
    class OsuToAlgorithm
    {
        public static BasicSongInfo Convert(Beatmap beatmap)
        {
            var ppq = 48;
            var timings = new List<TimingPoint>();
            double totalOffset = 0;
            OsuParsers.Beatmaps.Objects.TimingPoint last = null;

            /// the offset between the beginning of the song and the start of the note data in seconds.
            long startOffset = -1;
            foreach (var evt in beatmap.TimingPoints)
            {
                if (!evt.Inherited || evt.BeatLength < 0) continue;

                if (last == null)
                {
                    startOffset = evt.Offset;
                }
                else
                {
                    totalOffset += ppq * (evt.Offset - last.Offset) / last.BeatLength;
                }

                timings.Add(new TimingPoint
                {
                    SongTime = evt.Offset,
                    Meter = (int)evt.TimeSignature,
                    Tempo = evt.BeatLength,
                    Time = (long)Math.Round(totalOffset),
                });
                last = evt;
            }
            var noteTimes = new SortedSet<long>();

            foreach (var hit in beatmap.HitObjects)
            {
                //find the first timing point which is not less than this point
                var tpi = timings.LowerBound(x => !(x.SongTime < hit.StartTime));
                if (tpi == 0)
                {
                    //warn: ignore stuff thats not in time
                    continue;
                }
                tpi--;

                var tp = beatmap.TimingPoints[tpi];

                double beat = (hit.StartTime - tp.Offset) / tp.BeatLength;
                long tick = (long)Math.Round(beat * ppq);
                if (!noteTimes.Contains(tick))
                    noteTimes.Add(tick);
            }


            return new BasicSongInfo
            {
                AudioOffset = startOffset,
                PPQ = ppq,
                NoteTimes = noteTimes.ToList(),
                TimingPoints = timings,
            };
        }
    }
}
