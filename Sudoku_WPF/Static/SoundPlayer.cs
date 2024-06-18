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
    /// <summary>
    /// Class responsible for playing sounds and background music in the Sudoku application.
    /// </summary>
    internal static class SoundPlayer
    {
        private static MediaPlayer mediaPlayer; // Media player instance for background music
        private static BackgroundWorker soundWorker; // Background worker for sound operations
        private static bool isPlaying; // Flag indicating if music is currently playing

        /// <summary>
        /// Plays a sound from the specified file.
        /// </summary>
        /// <param name="soundName">The name of the sound file to play.</param>
        public static void PlaySound(string soundName)
        {
            if (Settings.soundOn)
            {
                
                try
                {
                    string soundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Sounds", $"{soundName}.wav");

                    if (File.Exists(soundPath))
                    {
                        MediaPlayer player = new MediaPlayer();
                        player.Open(new Uri(soundPath));
                        player.Play();
                    }
                    else
                    {
                        MessageBox.Show("Sound file not found: " + soundPath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error playing sound: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Starts playing background music from the specified path.
        /// </summary>
        /// <param name="path">The path of the music file to play.</param>
        public static void StartMusic(string musicName)
        {
            if (isPlaying) return; // If music is already playing, return

            string soundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Music", $"{musicName}.wav");

            mediaPlayer = new MediaPlayer();
            mediaPlayer.Open(new Uri(soundPath));
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded; // Add event handler for loop
            mediaPlayer.Play();

            isPlaying = true;
        }

        /// <summary>
        /// Stops playing the background music.
        /// </summary>
        public static void StopMusic()
        {
            if (!isPlaying) return; // If no music is playing, return

            isPlaying = false;
            mediaPlayer.Stop();
            mediaPlayer.MediaEnded -= MediaPlayer_MediaEnded; // Remove event handler
        }

        /// <summary>
        /// Event handler for when the media player ends playing the music, loops it back to the beginning.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private static void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            mediaPlayer.Position = TimeSpan.Zero; // Reset the position to the start
            mediaPlayer.Play(); // Start playing again
        }
    }
}
