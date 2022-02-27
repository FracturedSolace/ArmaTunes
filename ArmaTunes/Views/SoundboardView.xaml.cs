using ArmaTunes.Models;
using ArmaTunes.Models.WindowsLowLevel;
using ArmaTunes.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ArmaTunes.Views
{
    /// <summary>
    /// Interaction logic for SoundboardView.xaml
    /// </summary>
    public partial class SoundboardView : Window
    {
        private MainWindow parent;

        public SoundboardView(MainWindow parent)
        {
            this.parent = parent;

            InitializeComponent();
        }

        private void SoundFile_Clicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            string file = (string)button.Tag;

            var vlc = new VLCWrapper(file);

            //Stop the playlist while the soundboard plays
            parent.VLCPlaylist.Stop();
            //Play the sound and wait for it to finish
            vlc.LaunchAndWait(autoStop:true);
            //Restart the playlist
            parent.VLCPlaylist.Launch();


            this.Close();
        }

        private void PopulateSoundFiles()
        {
            int numClips = 0;

            foreach (string file in SoundboardViewModel.GetAllSoundFiles())
            {
                numClips++;

                string filename = System.IO.Path.GetFileName(file);

                var button = new Button()
                {
                    Content = new TextBlock()
                    {
                        Text = $"{filename}\n" +
                        $"[{numClips}]",
                        TextAlignment = TextAlignment.Center
                    },
                    Tag = file
                };

                button.Click += new RoutedEventHandler(SoundFile_Clicked);

                //Select the appropriate hotkey based on the position of the soundfile
                //Currently supports 0 through 10
                Nullable<VirtualKeyCodes> vk = null;

                switch (numClips)
                {
                    case 1:
                        vk = VirtualKeyCodes.VK_1;
                        break;
                    case 2:
                        vk = VirtualKeyCodes.VK_2;
                        break;
                    case 3:
                        vk = VirtualKeyCodes.VK_3;
                        break;
                    case 4:
                        vk = VirtualKeyCodes.VK_4;
                        break;
                    case 5:
                        vk = VirtualKeyCodes.VK_5;
                        break;
                    case 6:
                        vk = VirtualKeyCodes.VK_6;
                        break;
                    case 7:
                        vk = VirtualKeyCodes.VK_7;
                        break;
                    case 8:
                        vk = VirtualKeyCodes.VK_8;
                        break;
                    case 9:
                        vk = VirtualKeyCodes.VK_9;
                        break;
                    case 10:
                        vk = VirtualKeyCodes.VK_0;
                        break;
                }
                //Register the hotkey
                if (vk != null)
                {
                    new Hotkey(this, 5001 + numClips, KeyModifiers.None, vk.Value, (sender, e) =>
                    {
                        SoundFile_Clicked(button, new EventArgs());
                    });
                }

                pnlAudioFiles.Children.Add(button);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateSoundFiles();
        }
    }
}
