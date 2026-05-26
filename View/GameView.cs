using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimeTax.Model;
using TimeTax.Model.Entities;
using System;
using System.Collections.Generic;

namespace TimeTax.View
{
    public class GameView
    {
        private SpriteBatch spriteBatch;
        private Texture2D pixel;
        private SpriteFont font;
        private UIRenderer ui;

        private Microsoft.Xna.Framework.Vector2 playerPosition;
        private float currentTime;
        private int collectedCoins;
        private int requiredCoins;
        private string screenEffect = "normal";
        private bool doorOpen;
        private int currentScore;
        private string levelName = "";
        private int levelNumber = 1;

        private TimeManager? subscribedTimeManager;
        private Texture2D? background;

        public bool IsInPauseMenu => ui.IsInPauseMenu;

        private List<Platform> platforms = new();
        private List<FadingPlatform> fadingPlatforms = new();
        private List<Coin> coins = new();
        private List<Enemy> enemies = new();
        private List<Spike> spikes = new();
        private List<Checkpoint> checkpoints = new();
        private List<Portal> portals = new();
        private List<Conveyor> conveyors = new();
        private ExitDoor? door;
        private Player? player;

        private readonly HashSet<Coin> collectedCoinSet = new();
        private readonly HashSet<Checkpoint> activatedCheckpointSet = new();
        private readonly HashSet<FadingPlatform> invisiblePlatformSet = new();
        private int playerWidth = 20;
        private int playerHeight = 20;

        private bool gameOver;
        private bool levelCompleted;
        private bool gameWon;
        private bool isPaused;

        public GameView(GraphicsDevice graphicsDevice, SpriteBatch sharedSpriteBatch,
                        GameModel model, Texture2D sharedPixel, SpriteFont font,
                        Dictionary<string, Texture2D> backgrounds)
        {
            this.spriteBatch = sharedSpriteBatch;
            this.pixel = sharedPixel;
            this.font = font;
            this.background = backgrounds.ContainsKey("bg1") ? backgrounds["bg1"] : null;
            ui = new UIRenderer(sharedSpriteBatch, pixel, font);

            model.PlayerMoved += pos => playerPosition = new Microsoft.Xna.Framework.Vector2(pos.X, pos.Y);
            model.TimeManagerChanged += tm => SubscribeToTimeManager(tm);
            model.CoinsChanged += c => collectedCoins = c;
            model.ScoreChanged += s => currentScore = s;

            model.CoinCollectedEvent += coin => collectedCoinSet.Add(coin);
            model.CheckpointStateChanged += cp => activatedCheckpointSet.Add(cp);
            model.DoorStateChanged += open => doorOpen = open;
            model.FadingPlatformChanged += fp =>
            {
                if (fp.IsVisible)
                    invisiblePlatformSet.Remove(fp);
                else
                    invisiblePlatformSet.Add(fp);
            };

            model.BackgroundChanged += bgName =>
            {
                if (backgrounds.ContainsKey(bgName))
                    background = backgrounds[bgName];
            };

            model.LevelStarted += (num, level) =>
            {
                levelNumber = num;
                levelName = level.Name ?? "";
                requiredCoins = level.RequiredCoins;
                doorOpen = false;

                levelCompleted = false;
                gameOver = false;
                gameWon = false;
                isPaused = false;

                platforms = level.Platforms;
                fadingPlatforms = level.FadingPlatforms;
                coins = level.Coins;
                enemies = level.Enemies;
                spikes = level.Spikes;
                checkpoints = level.Checkpoints;
                portals = level.Portals;
                conveyors = level.Conveyors;
                door = level.Door;

                collectedCoinSet.Clear();
                activatedCheckpointSet.Clear();
                invisiblePlatformSet.Clear();
            };

            model.EnemiesChanged += newEnemies =>
            {
                enemies = newEnemies;
            };

            model.PlayerCreated += p =>
            {
                player = p;
                playerWidth = (int)p.Width;
                playerHeight = (int)p.Height;
            };
            model.PauseStateChanged += paused => isPaused = paused;
            model.GameLost += () => gameOver = true;
            model.GameWonEvent += () => gameWon = true;
            model.LevelCompletedEvent += () => levelCompleted = true;
        }

        private void SubscribeToTimeManager(TimeManager timeManager)
        {
            if (subscribedTimeManager != null)
            {
                subscribedTimeManager.TimeChanged -= OnTimeChanged;
                subscribedTimeManager.ScreenEffectChanged -= OnScreenEffectChanged;
            }

            subscribedTimeManager = timeManager;

            if (subscribedTimeManager != null)
            {
                subscribedTimeManager.TimeChanged += OnTimeChanged;
                subscribedTimeManager.ScreenEffectChanged += OnScreenEffectChanged;
            }
        }

        private void OnTimeChanged(float time)
        {
            currentTime = time;
        }

        private void OnScreenEffectChanged(string effect)
        {
            screenEffect = effect;
        }

        public void EnterPauseMenu()
        {
            ui.EnterPauseMenu();
        }

        public void ExitPauseMenu()
        {
            ui.ExitPauseMenu();
        }

        public void PauseMenuSelectNext()
        {
            ui.SelectNext();
        }

        public void PauseMenuSelectPrevious()
        {
            ui.SelectPrevious();
        }

        public int PauseMenuActivateSelected(bool soundEnabled)
        {
            return ui.ActivateSelected(soundEnabled);
        }

        public void UpdateSoundText(bool soundEnabled)
        {
            ui.UpdateSoundText(soundEnabled);
        }

        public void Draw(GameTime gameTime)
        {
            Color bgColor = screenEffect switch
            {
                "normal" => new Color(30, 30, 60),
                "orange" => new Color(180, 100, 20),
                "red" => new Color(180, 30, 30),
                "critical" => new Color(220, 20, 20),
                _ => Color.CornflowerBlue
            };

            if (screenEffect == "critical")
            {
                float pulse = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 8) * 0.3f + 0.7f;
                bgColor = new Color((int)(bgColor.R * pulse), (int)(bgColor.G * pulse), (int)(bgColor.B * pulse));
            }

            spriteBatch.Begin();

            if (background != null)
            {
                spriteBatch.Draw(background, new Rectangle(0, 0, 800, 480), Color.White);
                if (screenEffect != "normal")
                {
                    float alpha = screenEffect == "critical" ? 0.5f : 0.25f;
                    spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 480), bgColor * alpha);
                }
            }
            else
            {
                spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 480), bgColor);
            }

            foreach (var platform in platforms)
                spriteBatch.Draw(pixel,
                    new Rectangle((int)platform.Position.X, (int)platform.Position.Y, (int)platform.Width, (int)platform.Height),
                    Color.Gray);

            foreach (var fp in fadingPlatforms)
            {
                if (!invisiblePlatformSet.Contains(fp))
                {
                    float warning = fp.FadeTimer / FadingPlatform.VisibleDuration;
                    Color fpColor = warning > 0.7f
                        ? new Color(255, (int)(255 * (1 - warning)), 0)
                        : new Color(150, 150, 200);

                    spriteBatch.Draw(pixel,
                        new Rectangle((int)fp.Position.X, (int)fp.Position.Y, (int)fp.Width, (int)fp.Height),
                        fpColor);
                }
                else
                {
                    spriteBatch.Draw(pixel,
                        new Rectangle((int)fp.Position.X, (int)fp.Position.Y, (int)fp.Width, (int)fp.Height),
                        Color.Gray * 0.2f);
                }
            }

            foreach (var conveyor in conveyors)
            {
                Color conveyorColor = conveyor.Direction == ConveyorDirection.Right ? Color.Cyan : Color.LightBlue;
                spriteBatch.Draw(pixel,
                    new Rectangle((int)conveyor.Position.X, (int)conveyor.Position.Y, (int)conveyor.Width, (int)conveyor.Height),
                    conveyorColor);
                int arrowX = conveyor.Direction == ConveyorDirection.Right ? (int)(conveyor.Position.X + conveyor.Width - 10) : (int)conveyor.Position.X;
                spriteBatch.Draw(pixel, new Rectangle(arrowX, (int)conveyor.Position.Y - 3, 10, 4), Color.White);
            }

            foreach (var portal in portals)
            {
                Color portalColor = Color.Purple;
                float portalPulse = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 4) * 0.3f + 0.7f;
                spriteBatch.Draw(pixel,
                    new Rectangle((int)portal.Position.X, (int)portal.Position.Y, (int)portal.Width, (int)portal.Height),
                    new Color((int)(portalColor.R * portalPulse), (int)(portalColor.G * portalPulse), (int)(portalColor.B * portalPulse)));
            }

            foreach (var coin in coins)
            {
                if (collectedCoinSet.Contains(coin)) continue;
                Color coinColor = coin.Type == CoinType.Gold ? Color.Gold : Color.Yellow;
                float coinPulse = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 6) * 2f;
                int size = (int)(15 + coinPulse);
                int offset = (int)(coinPulse / 2);
                spriteBatch.Draw(pixel,
                    new Rectangle((int)coin.Position.X - offset, (int)coin.Position.Y - offset, size, size),
                    coinColor);
            }

            foreach (var enemy in enemies)
                DrawEnemy(enemy, gameTime);

            foreach (var spike in spikes)
                DrawSpike(spike);

            foreach (var cp in checkpoints)
            {
                Color cpColor = activatedCheckpointSet.Contains(cp) ? Color.LightGreen : Color.DarkGray;
                spriteBatch.Draw(pixel, new Rectangle((int)cp.Position.X, (int)cp.Position.Y, 4, 32), Color.Brown);
                spriteBatch.Draw(pixel, new Rectangle((int)cp.Position.X + 4, (int)cp.Position.Y, 20, 16), cpColor);
            }

            if (door != null)
            {
                Color doorColor = doorOpen ? Color.Green : Color.Red;
                spriteBatch.Draw(pixel,
                    new Rectangle((int)door.Position.X - 2, (int)door.Position.Y - 2,
                        (int)door.Width + 4, (int)door.Height + 4),
                    Color.DarkGray);
                spriteBatch.Draw(pixel,
                    new Rectangle((int)door.Position.X, (int)door.Position.Y,
                        (int)door.Width, (int)door.Height),
                    doorColor);
                spriteBatch.Draw(pixel,
                    new Rectangle((int)door.Position.X + 16, (int)door.Position.Y + 14, 4, 4),
                    Color.Yellow);
            }

            DrawPlayer(gameTime);

            ui.Draw(currentTime, collectedCoins, requiredCoins, currentScore, gameOver, levelCompleted, gameWon, isPaused, levelName, levelNumber);

            spriteBatch.End();
        }

        private void DrawPlayer(GameTime gameTime)
        {
            int x = (int)playerPosition.X;
            int y = (int)playerPosition.Y;
            int w = playerWidth;
            int h = playerHeight;

            spriteBatch.Draw(pixel, new Rectangle(x, y, w, h), Color.LimeGreen);
            spriteBatch.Draw(pixel, new Rectangle(x + 2, y + 2, w - 4, 10), Color.LightGreen);

            int eyeOffset = 12;
            spriteBatch.Draw(pixel, new Rectangle(x + eyeOffset, y + 5, 4, 4), Color.White);
            spriteBatch.Draw(pixel, new Rectangle(x + eyeOffset + 1, y + 6, 2, 2), Color.Black);

            spriteBatch.Draw(pixel, new Rectangle(x + 4, y + h - 4, 6, 4), Color.DarkGreen);
            spriteBatch.Draw(pixel, new Rectangle(x + w - 10, y + h - 4, 6, 4), Color.DarkGreen);

            spriteBatch.Draw(pixel, new Rectangle(x - 2, y + 14, 4, 10), Color.LimeGreen);
            spriteBatch.Draw(pixel, new Rectangle(x + w - 2, y + 14, 4, 10), Color.LimeGreen);
        }

        private void DrawEnemy(Enemy enemy, GameTime gameTime)
        {
            int x = (int)enemy.Position.X;
            int y = (int)enemy.Position.Y;
            int w = (int)enemy.Width;
            int h = (int)enemy.Height;

            spriteBatch.Draw(pixel, new Rectangle(x, y, w, h), Color.Red);
            spriteBatch.Draw(pixel, new Rectangle(x + 4, y + 4, 6, 6), Color.White);
            spriteBatch.Draw(pixel, new Rectangle(x + 14, y + 4, 6, 6), Color.White);
            spriteBatch.Draw(pixel, new Rectangle(x + 6, y + 6, 2, 2), Color.DarkRed);
            spriteBatch.Draw(pixel, new Rectangle(x + 16, y + 6, 2, 2), Color.DarkRed);
            spriteBatch.Draw(pixel, new Rectangle(x + 4, y - 4, 3, 4), Color.DarkRed);
            spriteBatch.Draw(pixel, new Rectangle(x + 17, y - 4, 3, 4), Color.DarkRed);
        }

        private void DrawSpike(Spike spike)
        {
            int x = (int)spike.Position.X;
            int y = (int)spike.Position.Y;
            int w = (int)spike.Width;
            int h = (int)spike.Height;

            for (int i = 0; i < w; i += 8)
            {
                int spikeHeight = Math.Min(8, w - i);
                spriteBatch.Draw(pixel,
                    new Rectangle(x + i, y + h - spikeHeight, 4, spikeHeight),
                    Color.DarkRed);
                spriteBatch.Draw(pixel,
                    new Rectangle(x + i + 2, y + h - spikeHeight - 2, 2, 2),
                    Color.Red);
            }
        }
    }
}