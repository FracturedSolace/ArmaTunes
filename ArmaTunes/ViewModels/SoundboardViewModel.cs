using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaTunes.ViewModels
{
    public static class SoundboardViewModel
    {
        public static string Default_Soundboard_Directory = @"D:\Music\Audio Clips";

        public static string[] GetAllSoundFiles()
        {
            List<string> found = new List<string>();
            foreach (string file in Directory.EnumerateFiles(Default_Soundboard_Directory))
            {
                string extension = Path.GetExtension(file);
                if (extension == ".mp3")
                    found.Add(file);
            }

            return found.ToArray();
        }
    }
}
