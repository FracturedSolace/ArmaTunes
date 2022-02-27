/*
Tools for launching Virtual Audio Cable (VAC) instances
-> These are used for routing audio signals
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaTunes.Models
{
    public static class LaunchVAC
    {
        public static string VACDirectory = @"D:\Programs\Virtual Audio Cable";
        public static string AudioRepeaterExeTitle = @"audiorepeater.exe";
        public static string AudioRepeaterKernelExeTitle = @"audiorepeater_ks.exe";

        public static EndPoint DefaultMicrophone = new EndPoint(@"Microphone (2- Logitech USB Hea");
        public static EndPoint ArtificialMicrophoneRelay = new EndPoint(@"Line 1 (Virtual Audio Cable)");
        public static EndPoint MusicRelay = new EndPoint(@"Line 2 (Virtual Audio Cable)");
        public static EndPoint SpeakerRelay = new EndPoint(@"Speakers (2- Logitech USB Heads");

        public class EndPoint
        {
            /*
             * Very generic wrapper class used to describe an audio device (EndPoint) in VAC's terminology
             */ 
            public string Name;

            public EndPoint(string Name)
            {
                this.Name = Name;
            }

            public override string ToString()
            {
                return this.Name;
            }
        }
    
        public class RelayCollection
        {
            public Relay Music2ArtificalMicrophone;
            public Relay Microphone2ArtificialMicrophone;
            public Relay Music2Speaker;
            public Relay ArtificialMicrophone2Speaker;

            public string ListeningTo
            {
                get
                {
                    if (ArtificialMicrophone2Speaker.Active)
                        return ArtificialMicrophone2Speaker.ToString();
                    else if (Music2Speaker.Active)
                        return Music2Speaker.ToString();
                    else
                        return "UNKNOWN";
                }
            }

            public RelayCollection()
            {
                LaunchAllRelays();
            }

            private void LaunchAllRelays()
            {
                Music2ArtificalMicrophone = new Relay(MusicRelay, ArtificialMicrophoneRelay, Title: "Music2ArtificalMicrophone");
                Microphone2ArtificialMicrophone = new Relay(DefaultMicrophone, ArtificialMicrophoneRelay, Title: "Microphone2ArtificialMicrophone");
                Music2Speaker = new Relay(MusicRelay, SpeakerRelay, Title: "Music");
                ArtificialMicrophone2Speaker = new Relay(ArtificialMicrophoneRelay, SpeakerRelay, AutoStart: false, Title: "Artificial Microphone");
            }

            public static void CloseAll()
            {
                using(System.Diagnostics.Process killProcess = new System.Diagnostics.Process())
                {
                    killProcess.StartInfo.FileName = "taskkill";
                    killProcess.StartInfo.Arguments = $"/f /im {AudioRepeaterExeTitle}";
                    killProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    killProcess.Start();
                    killProcess.WaitForExit();
                }
                
                //try
                //{
                //    Music2ArtificalMicrophone.Stop();
                //    Microphone2ArtificialMicrophone.Stop();
                //    Music2Speaker.Stop();
                //    ArtificialMicrophone2Speaker.Stop();
                //}
                //catch (System.InvalidOperationException) { }
            }
            public void Reset()
            {
                CloseAll();
                LaunchAllRelays();
            }

            public void ToggleOutput()
            {
                if (Music2Speaker.Active)
                {
                    Music2Speaker.Stop();
                    ArtificialMicrophone2Speaker.Start();
                }
                else
                {
                    Music2Speaker.Start();
                    ArtificialMicrophone2Speaker.Stop();
                }
            }
        }

        public class Relay
        {
            /*
             * Describes a VAC relay window which contains an input and an output
             */

            public readonly string Title;

            public readonly EndPoint Input;
            public readonly EndPoint Output;

            public bool Active
            {
                get
                {
                    return _active;
                }
            }

            private bool _active = false;
            private System.Diagnostics.Process Process;


            public Relay(EndPoint Input, EndPoint Output, string Title, bool AutoStart=true)
            {
                this.Input = Input;
                this.Output = Output;
                this.Title = Title;

                if (AutoStart)
                    this.Start();
            }

            public void Stop()
            {
                if (!this._active)
                    return;
                
                //using (System.Diagnostics.Process vacCloseProcess = new System.Diagnostics.Process())
                //{
                //    vacCloseProcess.StartInfo.FileName = $"{VACDirectory}\\{AudioRepeaterExeTitle}";
                //    vacCloseProcess.StartInfo.Arguments = $"/CloseInstance:\"{this.Title}\"";
                //    vacCloseProcess.Start();
                //}

                this.Process.Kill();
                this._active = false;
            }

            public void Start()
            {
                if (this._active)
                    return;

                this.Process = LaunchInstance(Input, Output, Title: this.Title);
                this._active = true;
            }

            public override string ToString()
            {
                return this.Title;
            }
        }

        public static System.Diagnostics.Process LaunchInstance(EndPoint InputEndpoint, EndPoint OutputEndpoint, bool AutoStart=true, string Title=null, bool Visible=false)
        {
            string ApplicationPath = VACDirectory + @"\" + AudioRepeaterExeTitle;
            string Arguments = $"/Input:\"{InputEndpoint}\" /Output:\"{OutputEndpoint}\"";

            //Append autostart, if the autostart parameter was passed
            if (AutoStart)
                Arguments += " /AutoStart";

            //Title the window, if a title was specified
            if (Title != null)
                Arguments += $" /WindowName:\"{Title}\"";

            Debug.Write($"Creating audio tunnel...");
            if (AutoStart)
                Debug.Write("AutoStarting audio tunnel...");
            
            System.Diagnostics.Process vacProcess = new System.Diagnostics.Process();
            vacProcess.StartInfo.FileName = ApplicationPath;
            vacProcess.StartInfo.Arguments = Arguments;
            if(!Visible)
                vacProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            vacProcess.Start();

            return vacProcess;
        }
    }
}
