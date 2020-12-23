
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
            double totalOffset = 0;//stores the number of ticks up to this event
            OsuParsers.Beatmaps.Objects.TimingPoint last = null;

            // how much later the evt is than the first event 
            long startOffset = -1;
            foreach (var evt in beatmap.TimingPoints)
            {
                if (!evt.Inherited || evt.BeatLength < 0) continue;

                if (last == null)
                {
                    startOffset = evt.Offset;

                    totalOffset = 0;
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
                //find the last timing point which is  less than or equal this point
                var tpi = timings.LowerBound(x => !(x.SongTime <= hit.StartTime))-1;
                if (tpi < 0)
                {
                    tpi = 0;
                    //warn: ignore stuff thats not in time
                    Console.WriteLine("Warning: delta="+(timings[tpi].SongTime-hit.StartTime));
                    continue;
                }

                var tp = timings[tpi];

                double beat = (hit.StartTime - tp.SongTime) / tp.Tempo;
                long tick = (long)Math.Round(beat * ppq) + tp.Time;
                if (!noteTimes.Contains(tick))
                    noteTimes.Add(tick);
            }


            return new BasicSongInfo
            {
                AudioOffset = startOffset+77,
                PPQ = ppq,
                NoteTimes = noteTimes.ToList(),
                TimingPoints = timings,
            };
        }
    }
}
