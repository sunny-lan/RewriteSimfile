using OsuParsers.Beatmaps;
using OsuParsers.Database;
using OsuParsers.Database.Objects;
using OsuParsers.Decoders;
using OsuSM.alg1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OsuSM
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
       // [STAThread]
        static void Main()
        {
            var osuPath = @"C:\Users\sunny\Programs\osu!\";
            var songsPath = Path.Join(osuPath, "Songs");
            var simPath = @"C:\Users\sunny\Programs\StepMania 5.1\Songs\Test";
            var osuDbPath = Path.Join(osuPath, "osu!.db");
            OsuDatabase db = DatabaseDecoder.DecodeOsu(osuDbPath);
            var songs = Directory.GetDirectories(songsPath);

            Dictionary<int, List<DbBeatmap>> bms=db.Beatmaps
                .GroupBy(x => x.BeatmapSetId, x => x)
                .ToDictionary(x => x.Key, x => x.ToList());

            foreach (var songPath in songs)
            {

                var songFolder =Path.GetFileName(songPath);
                Console.WriteLine(songFolder);


                var maps = Directory.GetFiles(Path.Join(songsPath, songFolder), "*.osu");
                Simfile s=null;

                List<DbBeatmap> bs=null;
                

                foreach (var mapPath in maps)
                {
                    var songName = Path.GetFileNameWithoutExtension(mapPath);
                    Console.WriteLine(" " + songName);
                    Beatmap beatmap = BeatmapDecoder.Decode(Path.Join(songsPath, songFolder, songName + ".osu"));

                    if (bs == null)
                    {
                        bs = bms[beatmap.MetadataSection.BeatmapSetID];
                        bs.Sort((x, y) => x.CirclesCount
                        .CompareTo(y.CirclesCount));
                    }

                    BasicSongInfo songInfo = OsuToAlgorithm.Convert(beatmap);
                    Random rng = new Random(beatmap.MetadataSection.BeatmapID);
                    StepScoreGenerator sg = new StepScoreGenerator
                    {
                        Rng = rng,
                    };
                    Algorithm a = new Algorithm
                    {
                        Info = songInfo,
                        RecoveryInterval = songInfo.PPQ / 2 / 8,
                        StepScore = sg.RandomStepScore(songInfo),
                    };
                    if (s == null)
                    {
                        s = AlgorithmToSimfile.Convert(beatmap, songInfo, new List<Simfile.Chart>());
                    }

                    var x = a.Run();
                    var chart = AlgorithmToSimfile.ConvertChart(beatmap, songInfo, x,
                        diff: bs.FindIndex(y=>y.BeatmapId==beatmap.MetadataSection.BeatmapID)
                        
                    );
                    s.Charts.Add(chart);

                }
                Directory.CreateDirectory(Path.Join(simPath, songFolder));

                var sb = new StringBuilder();
                s.Append(sb);
                File.WriteAllText(Path.Join(simPath, songFolder, songFolder + ".sm"), sb.ToString());
        
                File.Copy(Path.Join(songsPath, songFolder, s.Music),
                    Path.Join(simPath, songFolder, s.Music), true);
                
            }
            //Application.SetHighDpiMode(HighDpiMode.SystemAware);
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
        }
    }
}
