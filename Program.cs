using OsuParsers.Beatmaps;
using OsuParsers.Decoders;
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
            var osuPath = @"C:\Users\sunny\Programs\osu!\Songs\";
            var songFolder = @"1178477 Umino Koukou Teibo-bu - SEA HORIZON (TV Size)";
            var songName = @"Umino Koukou Teibo-bu - SEA HORIZON (TV Size) (Nabi-) [Hard]";
            Beatmap beatmap = BeatmapDecoder.Decode(Path.Join(osuPath, songFolder, songName+".osu"));
            BasicSongInfo songInfo = OsuToAlgorithm.Convert(beatmap);
            Random rng = new Random(2);
            StepScoreGenerator sg = new StepScoreGenerator {
                Rng = rng,
            };
            Algorithm a = new Algorithm
            {
                Info = songInfo,
                RecoveryInterval = songInfo.PPQ / 2/8,
                StepScore=sg.RandomStepScore(songInfo),
            };
            var x = a.Run();
            var simfile= AlgorithmToSimfile.Convert( beatmap, songInfo,x );
            var sb = new StringBuilder();
            simfile.Append(sb);
            var simPath = @"C:\Users\sunny\Programs\StepMania 5.1\Songs\Test";
            Directory.CreateDirectory(Path.Join(simPath,songFolder));
            File.WriteAllText(Path.Join(simPath,songFolder, songName+".sm"), sb.ToString());
            File.Copy(Path.Join(osuPath, songFolder, beatmap.GeneralSection.AudioFilename),
                Path.Join(simPath, songFolder, beatmap.GeneralSection.AudioFilename), true);
            //Application.SetHighDpiMode(HighDpiMode.SystemAware);
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
        }
    }
}
