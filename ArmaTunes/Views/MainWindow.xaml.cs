using ArmaTunes.Models;
using ArmaTunes.Models.WindowsLowLevel;
using ArmaTunes.Views;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static ArmaTunes.Models.LaunchVAC;

namespace ArmaTunes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public RelayCollection Relays = null;
        public VLCWrapper VLCPlaylist = new VLCWrapper(VLCWrapper.Default_Playlist);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpdateButtonText()
        {
            //Update btnToggleMusic
            if (Relays.Music2ArtificalMicrophone.Active)
                txtMusicEnabled.Text = "[ON]";
            else
                txtMusicEnabled.Text = "[OFF]";

            //Update btnToggleOutput
            txtListeningTo.Text = $"[{Relays.ListeningTo}]";

            //Update btnMicPassThrough
            if (Relays.Microphone2ArtificialMicrophone.Active)
                txtMicPassThrough.Text = "[ON]";
            else
                txtMicPassThrough.Text = "[OFF]";
        }

        public void ToggleMusic()
        {
            if (Relays == null)
                return;

            if (Relays.Music2ArtificalMicrophone.Active)
            {
                Relays.Music2ArtificalMicrophone.Stop();
                Relays.Music2Speaker.Stop();
            }
            else
            {
                Relays.Music2ArtificalMicrophone.Start();
                Relays.Music2Speaker.Start();
            }

            UpdateButtonText();
        }

        private void ToggleMicPassthrough()
        {
            //Skip this function altogether if the relays haven't been setup yet
            if (Relays == null)
                return;

            if (Relays.Microphone2ArtificialMicrophone.Active)
            {
                Relays.Microphone2ArtificialMicrophone.Stop();
            }
            else
            {
                Relays.Microphone2ArtificialMicrophone.Start();
            }

            UpdateButtonText();
        }

        private void EnableButtons()
        {
            btnToggleMusic.IsEnabled = true;
            btnToggleOutput.IsEnabled = true;
            btnMicPassthrough.IsEnabled = true;
        }

        private void btnDefaultInstances_Click(object sender, RoutedEventArgs e)
        {
            Relays = new RelayCollection();

            EnableButtons();

            UpdateButtonText();

            //Set the button to now REPLACE instances
            ((Button)sender).Content = "Restart Default Instances";
            ((Button)sender).Click -= btnDefaultInstances_Click;
            ((Button)sender).Click += btnRestartInstances_Click;
        }

        private void btnRestartInstances_Click(object sender, RoutedEventArgs e)
        {
            Relays.Reset();
            UpdateButtonText();
        }

        private void btnToggleMusic_Click(object sender, RoutedEventArgs e)
        {
            ToggleMusic();
        }

        private void BtnToggleOutput_Click(object sender, RoutedEventArgs e)
        {
            Relays.ToggleOutput();

            UpdateButtonText();
        }

        private void BtnMicPassthrough_Click(object sender, RoutedEventArgs e)
        {
            ToggleMicPassthrough();
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Relays?.CloseAll();
            VLCPlaylist.Stop();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RegisterHotkeys();
        }

        private void RegisterHotkeys()
        {
            //Associate the hotkey for music output toggle to CAPS
            new Hotkey(this, 9019, KeyModifiers.None, VirtualKeyCodes.VK_CAPITAL, (object sender2, EventArgs e2) =>
            {
                ToggleMusic();
            });

            //Associate the hotkey for mic passthrough to TILDE
            new Hotkey(this, 9020, KeyModifiers.None, VirtualKeyCodes.VK_OEM_3, (object sender2, EventArgs e2) =>
            {
                ToggleMicPassthrough();
            });

            //Associate the hotkey for the soundboard
            new Hotkey(this, 9021, KeyModifiers.Control | KeyModifiers.Shift, VirtualKeyCodes.VK_SHIFT, (object sender2, EventArgs e2) =>
            {
                LaunchSoundboard();
            });
        }

        private void LaunchSoundboard()
        {
            var window = new SoundboardView();
            window.Show();
        }

        private void btnLaunchPlaylist_Click(object sender, RoutedEventArgs e)
        {
            VLCPlaylist.Launch();
        }

        private void BtnLaunchSoundboard_Click(object sender, RoutedEventArgs e)
        {
            LaunchSoundboard();
        }
    }
}
