using System;
using System.Collections.Generic;
using System.Text;

namespace OsuSM
{
    class Simfile
    {
        public class Chart
        {
            public long PPQ;

            public class NoteLine
            {
                /// <summary>
                /// Time, in PPQ
                /// </summary>
                public long Time;

                /// <summary>
                /// Note types specified in https://github.com/stepmania/stepmania/wiki/sm
                /// 0 - no note
                /// 1 - normal note
                /// </summary>
                public int[] Notes;

                public override string ToString()
                {
                    return string.Concat(Notes);
                }
            }



            public string Type ;
            public string Maker;
            public string Difficulty;
            public int Level;
            public List<NoteLine> Lines;

            private static readonly long[] validNoteTypes= { 4,8,12,16,24,32,48,64,192};

            public void Append(StringBuilder sb)
            {
                sb.AppendLine("#NOTES:");

                sb.Append("    ");
                sb.Append(Type);
                sb.AppendLine(":");


                sb.Append("    ");
                sb.Append(Maker);
                sb.AppendLine(":");

                sb.Append("    ");
                sb.Append(Difficulty);
                sb.AppendLine(":");


                sb.Append("    ");
                sb.Append(Level);
                sb.AppendLine(":");

                sb.AppendLine("    0,0,0,0,0:");

                int idx = 0;
                long barEnd=0;
                long barLen = PPQ * 4;
                long last = validNoteTypes[validNoteTypes.Length - 1];
                while (idx < Lines.Count)
                {
                    barEnd +=barLen;
                    int st = idx;

                    //max PPQ per division able to fit all lines
                    long minDivLen= barLen / validNoteTypes[0];

                    while(idx<Lines.Count && Lines[idx].Time<barEnd)
                    {
                        foreach(long noteType in validNoteTypes)
                        {
                            //PPQ per division
                            long divLen = barLen / noteType;

                            if (Lines[idx].Time % divLen == 0 || noteType==last)
                            {
                                minDivLen = divLen;
                                break;
                            }
                        }

                        idx++;
                    }

                    //number of divisions per bar
                    long divs = barLen / minDivLen;
                    
                    string[] res = new string[divs];
                    Array.Fill(res, "0000");
                    for(int j = st; j < idx; j++)
                    {
                        res[(Lines[j].Time-Lines[st].Time) / minDivLen] = Lines[j].ToString();
                    }

                    foreach(string s in res)
                        sb.AppendLine(s);

                    if (idx == Lines.Count)
                        sb.AppendLine(";");
                    else
                        sb.AppendLine(",");
                }
            }
        }
        public class TimingPoint
        {
            /// <summary>
            /// the beat on which it changes
            /// </summary>
            public double Beat;

            /// <summary>
            /// in beats per minute
            /// </summary>
            public double BPM;
            public void Append(StringBuilder sb)
            {
                sb.Append(Beat);
                sb.Append('=');
                sb.Append(BPM);
            }
        }

        /// <summary>
        /// Path to the audio file
        /// </summary>
        public string Music;

        public string Title, Artist;

        /// <summary>
        /// the offset between the beginning of the song and the start of the note data in seconds.
        /// </summary>
        public double Offset;

        public List<Chart> Charts;
        public List<TimingPoint> BPMs;


        public void Append(StringBuilder sb)
        {
            sb.Append("#TITLE:");
            sb.Append(Title);
            sb.AppendLine(";");

            sb.Append("#ARTIST:");
            sb.Append(Artist);
            sb.AppendLine(";");

            sb.Append("#MUSIC:");
            sb.Append(Music);
            sb.AppendLine(";");



            sb.Append("#OFFSET:");
            sb.Append(Offset);
            sb.AppendLine(";");

            sb.Append("#BPMS:");
            for(int i = 0; i < BPMs.Count; i++)
            {
                BPMs[i].Append(sb);
                if (i != BPMs.Count - 1)
                    sb.AppendLine(",");
            }
            sb.AppendLine(";");

            foreach(var chart in Charts)
            {
                chart.Append(sb);
            }
        }
    }
}
