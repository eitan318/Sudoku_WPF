using Sudoku_WPF.publico;
using System;
using System.ComponentModel;
using System.IO;
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
            soundWorker = new BackgroundWorker();
            soundWorker.DoWork += SoundWorker_DoWork;
            soundWorker.RunWorkerCompleted += SoundWorker_RunWorkerCompleted;

            isPlaying = true;
            soundWorker.RunWorkerAsync();
        }

        public static void StopMusic()
        {
            if (!isPlaying) return;

            isPlaying = false;
            mediaPlayer.Stop();
        }

        private static void SoundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (isPlaying)
            {
                mediaPlayer.Dispatcher.Invoke(() => mediaPlayer.Play());
                Thread.Sleep(100); // Small delay to prevent CPU spinning
            }
        }

        private static void SoundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            mediaPlayer.Close();
        }
    }
}
