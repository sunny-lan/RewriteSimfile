using OsuParsers.Beatmaps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OsuSM
{
    class AlgorithmToSimfile
    {
        public static Simfile Convert(
            Beatmap bm,
            BasicSongInfo info,
            Move[] moves
        )
        {
            var lines = new List<Simfile.Chart.NoteLine>();
            for(int i = 0; i < moves.Length; i++)
            {
                int[] notes = new int[4];
                for(int foot=0;foot<2;foot++)
                    if (moves[i].Pos(foot) != -1) notes[moves[i].Pos(foot)] = 1;
                lines.Add(new Simfile.Chart.NoteLine { 
                    Time=info.NoteTimes[i],
                    Notes=notes,
                });
            }
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
                Charts = new List<Simfile.Chart>{
                    new Simfile.Chart {
                        Type="dance-single",
                        Maker="Easy",
                        Difficulty="Beginner",
                        Level=9,
                        Lines=lines,
                        PPQ=info.PPQ,
                    }
                },
            };
        }
    }
}
