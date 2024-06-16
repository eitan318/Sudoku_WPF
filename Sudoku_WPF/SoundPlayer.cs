using Sudoku_WPF.publico;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using static Sudoku_WPF.publico.Constants;

namespace Sudoku_WPF
{
    internal class SoundPlayer
    {
        private static MediaPlayer mediaPlayer;
        private static BackgroundWorker soundWorker;
        private static bool isPlaying;

        public static void PlaySound(string soundName)
        {
            if (Settings.soundOn)
            {
                string relativePath = $"\\Assets\\Sounds\\{soundName}.wav";
                try
                {
                    // Combine the base directory and the relative path
                    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string absolutePath = baseDirectory.Substring(0, baseDirectory.IndexOf(Constants.PROJ_DIRECTORY) + Constants.PROJ_DIRECTORY.Length) + relativePath;

                    // Ensure the file exists before playing
                    if (File.Exists(absolutePath))
                    {
                        MediaPlayer player = new MediaPlayer();
                        player.Open(new Uri(absolutePath));
                        player.Play();
                    }
                    else
                    {
                        MessageBox.Show("Sound file not found: " + absolutePath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error playing sound: " + ex.Message);
                }
            }
        }

        public static void StartMusic(string path)
        {
            if (isPlaying) return;

            mediaPlayer = new MediaPlayer();
            mediaPlayer.Open(new Uri(path));
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded; // Add event handler for loop
            mediaPlayer.Play();

            isPlaying = true;
        }

        public static void StopMusic()
        {
            if (!isPlaying) return;

            isPlaying = false;
            mediaPlayer.Stop();
            mediaPlayer.MediaEnded -= MediaPlayer_MediaEnded; // Remove event handler
        }

        private static void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            mediaPlayer.Position = TimeSpan.Zero; // Reset the position to the start
            mediaPlayer.Play(); // Start playing again
        }
    }
}
