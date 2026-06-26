using System;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace PROG6221_V1
{
    public static class AudioPlayer
    {
        public static void PlayGreeting(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Audio file not found: " + filePath);
                return;
            }

            try
            {
                SoundPlayer player = new SoundPlayer(filePath);
                player.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Audio error: " + ex.Message);
            }
        }
    }
}