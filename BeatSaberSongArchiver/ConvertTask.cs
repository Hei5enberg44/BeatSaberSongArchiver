using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NVorbis;
using CSAudioConverter;

namespace BeatSaberSongArchiver
{
    class ConvertTask
    {
        public string sourceFile;
        public string destinationFile;
        public VorbisReader vorbis;
        public AudioConverter audioConverter;
        public bool finished;

        public ConvertTask(string sourceFile)
        {
            this.sourceFile = sourceFile;
            this.finished = false;
        }
    }
}
