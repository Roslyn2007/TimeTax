using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Diagnostics;

namespace TimeTax.View
{
    public class AudioManager
    {
        private Song? menuMusic, gameMusic, victoryMusic, gameOverMusic;
        private Song? levelCompleteMusic;
        private SoundEffect? coin, jump, hurt, checkpoint, portal, doorOpen, levelTransition;
        private string? currentSongName;
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
            victoryMusic      = TryLoadSong(content, "music/victory");
            gameOverMusic     = TryLoadSong(content, "music/gameover");

            if (victoryMusic == null && gameMusic != null)
            {
                Debug.WriteLine("[Audio] victory.mp3 not found, using background as fallback");
                victoryMusic = gameMusic;
            }
            if (gameOverMusic == null && gameMusic != null)
            {
                Debug.WriteLine("[Audio] gameover.mp3 not found, using background as fallback");
                gameOverMusic = gameMusic;
            }

            coin       = TryLoadSfx(content, gd, "sfx/coin");
            jump       = TryLoadSfx(content, gd, "sfx/jump");
            hurt       = TryLoadSfx(content, gd, "sfx/hurt");
            checkpoint = TryLoadSfx(content, gd, "sfx/checkpoint");
            portal     = TryLoadSfx(content, gd, "sfx/portal");
            doorOpen   = TryLoadSfx(content, gd, "sfx/door_open");
            levelTransition = TryLoadSfx(content, gd, "sfx/level_transition");

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.6f * MasterVolume;
        }

        private static Song? TryLoadSong(ContentManager c, string path)
        {
            try
            {
                var song = c.Load<Song>(path);
                Debug.WriteLine($"[Audio] Loaded song: {path}");
                return song;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Audio] FAILED to load song {path}: {ex.Message}");
                return null;
            }
        }

        private static SoundEffect? TryLoadSfx(ContentManager c, GraphicsDevice? gd, string path)
        {
            try { return c.Load<SoundEffect>(path); }
            catch { Debug.WriteLine($"[Audio] FAILED to load sfx {path} from Content"); }

            if (gd == null) return null;

            string wavPath = Path.Combine(c.RootDirectory, path + ".wav");
            if (!File.Exists(wavPath)) return null;

            try
            {
                using var stream = File.OpenRead(wavPath);
                return SoundEffect.FromStream(stream);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Audio] FAILED to load sfx {wavPath}: {ex.Message}");
                return null;
            }
        }

        public void PlayMenuMusic()        => PlaySong("menu", menuMusic, repeat: true);
        public void PlayGameMusic()        => PlaySong("game", gameMusic, repeat: true);
        public void PlayLevelCompleteMusic() => PlaySong("levelcomplete", levelCompleteMusic, repeat: true);
        public void PlayVictoryMusic()     => PlaySong("victory", victoryMusic, repeat: true);
        public void PlayGameOverMusic()    => PlaySong("gameover", gameOverMusic, repeat: true);
        public void PlayVictoryMusicOnce() => PlaySong("victory", victoryMusic, repeat: false);
        public void PlayGameOverMusicOnce() => PlaySong("gameover", gameOverMusic, repeat: false);

        private void PlaySong(string name, Song? song, bool repeat = true)
        {
            if (!SoundEnabled)
            {
                Debug.WriteLine($"[Audio] PlaySong({name}) skipped - sound disabled");
                return;
            }
            if (song == null)
            {
                Debug.WriteLine($"[Audio] PlaySong({name}) skipped - song is null");
                return;
            }
            if (currentSongName == name && MediaPlayer.State == MediaState.Playing)
            {
                Debug.WriteLine($"[Audio] PlaySong({name}) skipped - already playing");
                return;
            }

            if (currentSongName == name && MediaPlayer.State == MediaState.Paused)
            {
                Debug.WriteLine($"[Audio] PlaySong({name}) resuming paused track");
                MediaPlayer.Resume();
                return;
            }

            Debug.WriteLine($"[Audio] Playing song: {name} (repeat={repeat})");
            MediaPlayer.IsRepeating = repeat;
            MediaPlayer.Play(song);
            currentSongName = name;
        }

        public void StopMusic()
        {
            Debug.WriteLine("[Audio] StopMusic() called");
            MediaPlayer.Stop();
            currentSongName = null;
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
        public void PlayDoorOpen()   { if (SoundEnabled) doorOpen?.Play(sfxVol * MasterVolume, 0, 0); }
        public void PlayLevelTransition() { if (SoundEnabled) levelTransition?.Play(sfxVol * MasterVolume, 0, 0); }
    }
}