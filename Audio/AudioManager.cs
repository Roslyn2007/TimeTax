using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace TimeTax.View
{
    public class AudioManager
    {
        private Song? menuMusic, gameMusic, victoryMusic, gameOverMusic;
        private Song? levelCompleteMusic;
        private SoundEffect? coin, jump, hurt, checkpoint, portal;
        private Song? currentSong;
        private float sfxVol = 0.7f;
        private GraphicsDevice? graphicsDevice;

        public bool SoundEnabled { get; set; } = true;
        public float MasterVolume { get; set; } = 1.0f;

        public void LoadContent(ContentManager content, GraphicsDevice? gd = null)
        {
            this.graphicsDevice = gd;

            menuMusic         = TryLoadSong(content, "music/menu");
            gameMusic         = TryLoadSong(content, "music/background");
            levelCompleteMusic = TryLoadSong(content, "music/levelcomplete");
            victoryMusic      = TryLoadSong(content, "music/background");
            gameOverMusic     = TryLoadSong(content, "music/background");

            coin       = TryLoadSfx(content, gd, "sfx/coin");
            jump       = TryLoadSfx(content, gd, "sfx/jump");
            hurt       = TryLoadSfx(content, gd, "sfx/hurt");
            checkpoint = TryLoadSfx(content, gd, "sfx/checkpoint");
            portal     = TryLoadSfx(content, gd, "sfx/portal");

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.6f * MasterVolume;
        }

        private static Song? TryLoadSong(ContentManager c, string path)
        {
            try { return c.Load<Song>(path); }
            catch { return null; }
        }

        private static SoundEffect? TryLoadSfx(ContentManager c, GraphicsDevice? gd, string path)
        {
            try { return c.Load<SoundEffect>(path); }
            catch { }

            if (gd == null) return null;

            string wavPath = Path.Combine(c.RootDirectory, path + ".wav");
            if (!File.Exists(wavPath)) return null;

            try
            {
                using var stream = File.OpenRead(wavPath);
                return SoundEffect.FromStream(stream);
            }
            catch { return null; }
        }

        public void PlayMenuMusic()        => PlaySong("menu", menuMusic);
        public void PlayGameMusic()        => PlaySong("game", gameMusic);
        public void PlayLevelCompleteMusic() => PlaySong("levelcomplete", levelCompleteMusic);
        public void PlayVictoryMusic()     => PlaySong("victory", victoryMusic);
        public void PlayGameOverMusic()    => PlaySong("gameover", gameOverMusic);

        private void PlaySong(string name, Song? song)
        {
            if (!SoundEnabled || song == null) return;
            if (currentSong == song && MediaPlayer.State == MediaState.Playing) return;

            if (currentSong == song && MediaPlayer.State == MediaState.Paused)
            {
                MediaPlayer.Resume();
                return;
            }

            MediaPlayer.Play(song);
            currentSong = song;
        }

        public void StopMusic()
        {
            MediaPlayer.Stop();
            currentSong = null;
        }

        public void PauseMusic()
        {
            if (MediaPlayer.State == MediaState.Playing)
                MediaPlayer.Pause();
        }

        public void ResumeMusic()
        {
            if (MediaPlayer.State == MediaState.Paused)
                MediaPlayer.Resume();
        }

        public void ApplyVolume()
        {
            MediaPlayer.Volume = SoundEnabled ? 0.6f * MasterVolume : 0f;
            if (!SoundEnabled)
                StopMusic();
        }

        public void ToggleSound()
        {
            SoundEnabled = !SoundEnabled;
            ApplyVolume();
        }

        public void PlayCoin()       { if (SoundEnabled) coin?.Play(sfxVol * MasterVolume, 0, 0); }
        public void PlayJump()       { if (SoundEnabled) jump?.Play(sfxVol * MasterVolume, 0, 0); }
        public void PlayHurt()       { if (SoundEnabled) hurt?.Play(sfxVol * MasterVolume, 0, 0); }
        public void PlayCheckpoint() { if (SoundEnabled) checkpoint?.Play(sfxVol * MasterVolume, 0, 0); }
        public void PlayPortal()     { if (SoundEnabled) portal?.Play(sfxVol * MasterVolume, 0, 0); }
    }
}