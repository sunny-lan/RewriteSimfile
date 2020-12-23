using OsuParsers.Beatmaps;
using OsuSM.alg1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OsuSM
{
    class AlgorithmToSimfile
    {
        static string[] diffs = { "Beginner","Easy","Medium","Hard","Challenge","Edit"};
        public static Simfile.Chart ConvertChart(
            Beatmap bm,
            BasicSongInfo info,
            Move[] moves,
            int diff)
        {
            var lines = new List<Simfile.Chart.NoteLine>();
            for (int i = 0; i < moves.Length; i++)
            {
                int[] notes = new int[4];
                for (int foot = 0; foot < 2; foot++)
                    if (moves[i].Pos(foot) != -1) notes[moves[i].Pos(foot)] = 1;
                lines.Add(new Simfile.Chart.NoteLine
                {
                    Time = info.NoteTimes[i],
                    Notes = notes,
                });
            }
            return new Simfile.Chart
            {
                Type = "dance-single",
                Maker = bm.MetadataSection.Version,
                Difficulty =diffs[Math.Min(diff, diffs.Length-1)],
                
                Level = diff+5,
                Lines = lines,
                PPQ = info.PPQ,
            };
        }

        public static Simfile Convert(
            Beatmap bm,
            BasicSongInfo info,
            List<Simfile.Chart> charts
        )
        {
            return new Simfile
            {
                Artist = bm.MetadataSection.Artist,
                Music = bm.GeneralSection.AudioFilename,
                Offset = -info.AudioOffset / 1000d,
                Title = bm.MetadataSection.Title,
                BPMs = info.TimingPoints.ConvertAll(x => new Simfile.TimingPoint
                {
                    Beat = x.Time / (double)info.PPQ,
                    BPM = 60*1000.0 / x.Tempo,
                }),
                Charts = charts,
            };
        }
    }
}
