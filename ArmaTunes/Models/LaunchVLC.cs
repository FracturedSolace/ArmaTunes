using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaTunes.Models
{
    public class VLCWrapper
    {
        public readonly string file;

        public static string VLC_Dir = @"D:\Programs\VLC\vlc.exe";
        public static string Default_Audio_Device = "Line 2 (Virtual Audio Cable) ($1,$64)";
        public static string Default_Playlist = @"D:\music\playlists\Arma 3 Helicopter Playlist.xspf";

        public Process VLCProcess;

        public VLCWrapper(string file)
        {
            if (file == "")
            {
                throw new Exception("File should not be blank on VLCWrapper creation");
            }

            this.file = file;
        }

        public void Launch(bool autoStop=false)
        {
            string arguments = $"\"{this.file}\" --aout=waveout --waveout-audio-device=\"{Default_Audio_Device}\"";

            if (autoStop)
                arguments += " --play-and-exit --no-repeat";
            else
                arguments += " --random";

            this.VLCProcess = new Process();
            VLCProcess.StartInfo.FileName = VLC_Dir;
            VLCProcess.StartInfo.Arguments = arguments;
            VLCProcess.Start();
        }

        public void LaunchAndWait(bool autoStop = false)
        {
            this.Launch(autoStop);
            VLCProcess.WaitForExit();
        }

        public void Stop()
        {
            if(VLCProcess != null && VLCProcess.StartTime != null)
                VLCProcess.Kill();
        }
    }
}
