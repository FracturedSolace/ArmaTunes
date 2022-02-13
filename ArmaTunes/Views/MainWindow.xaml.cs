using ArmaTunes.Models;
using ArmaTunes.Models.WindowsLowLevel;
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
        }

        private void btnDefaultInstances_Click(object sender, RoutedEventArgs e)
        {
            Relays = new RelayCollection();

            btnToggleMusic.IsEnabled = true;
            btnToggleOutput.IsEnabled = true;

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

        private void btnToggleMusic_Click(object sender, RoutedEventArgs e)
        {
            ToggleMusic();
        }

        private void BtnToggleOutput_Click(object sender, RoutedEventArgs e)
        {
            Relays.ToggleOutput();

            UpdateButtonText();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Relays.CloseAll();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Hotkey hk = new Hotkey(this, 9049);
            hk.OnHotKeyPressed += (object sender2, EventArgs e2) =>
            {
                Debug.Write("Hotkey pressed!");
            };
        }
    }
}
