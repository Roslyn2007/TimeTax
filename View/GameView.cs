using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimeTax.Model;
using TimeTax.Model.Entities;
using System;

namespace TimeTax.View
{
    public class GameView
    {
        private SpriteBatch spriteBatch;
        private Texture2D pixel;
        private SpriteFont font;
        private UIRenderer ui;

        private float animationTimer = 0f;
        private int currentFrame = 0;
        private const float FrameDuration = 0.15f;

        private Microsoft.Xna.Framework.Vector2 playerPosition;
        private float currentTime;
        private int collectedCoins;
        private int requiredCoins;
        private string screenEffect = "normal";
        private bool doorOpen;
        private int currentScore;
        private string levelName = "";
        private int levelNumber = 1;

        private GameModel model;

        public GameView(GraphicsDevice graphicsDevice, SpriteBatch sharedSpriteBatch, GameModel model, Texture2D sharedPixel, SpriteFont font)
        {
            this.spriteBatch = sharedSpriteBatch;
            this.model = model;
            this.pixel = sharedPixel;
            this.font = font;
            ui = new UIRenderer(sharedSpriteBatch, pixel, font);

            model.PlayerMoved += pos => playerPosition = new Microsoft.Xna.Framework.Vector2(pos.X, pos.Y);
            
            // Подписка на TimeChanged через метод, чтобы можно было переподписаться
            SubscribeToTimeManager(model.Time);
            
            model.CoinsChanged += c => collectedCoins = c;
            model.ScoreChanged += s => currentScore = s;
            
            model.LevelStarted += num =>
            {
                levelNumber = num;
                levelName = model.CurrentLevel?.Name ?? "";
                requiredCoins = model.TotalCoinsRequired;
                doorOpen = false;
            };
            
            // === НОВОЕ: переподписка при смене TimeManager ===
            model.TimeManagerChanged += newTimeManager =>
            {
                SubscribeToTimeManager(newTimeManager);
            };
            
            model.Time.TimeRanOut += () => { };
            model.LevelFinished += () => { };

            playerPosition = new Microsoft.Xna.Framework.Vector2(model.Player.Position.X, model.Player.Position.Y);
            currentTime = model.Time.CurrentTime;
            collectedCoins = model.CollectedCoins;
            requiredCoins = model.TotalCoinsRequired;
            doorOpen = model.CurrentLevel.Door?.IsOpen ?? false;
            levelName = model.CurrentLevel?.Name ?? "";
            levelNumber = model.CurrentLevelNumber;
        }

        private void SubscribeToTimeManager(TimeManager timeManager)
        {
            // Отписываемся от старого, если нужно (здесь упрощённо — просто подписываемся на новый)
            timeManager.TimeChanged += t => currentTime = t;
            timeManager.ScreenEffectChanged += eff => screenEffect = eff;
        }

        public void Draw(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            UpdateAnimation(deltaTime);

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

            spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 480), bgColor);

            foreach (var platform in model.CurrentLevel.Platforms)
                spriteBatch.Draw(pixel,
                    new Rectangle((int)platform.Position.X, (int)platform.Position.Y, (int)platform.Width, (int)platform.Height),
                    Color.Gray);

            foreach (var fp in model.CurrentLevel.FadingPlatforms)
            {
                if (fp.IsVisible)
                {
                    float warning = fp.FadeTimer / Level.FadingPlatform.VisibleDuration;
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

            foreach (var conveyor in model.CurrentLevel.Conveyors)
            {
                Color conveyorColor = conveyor.Direction == ConveyorDirection.Right ? Color.Cyan : Color.LightBlue;
                spriteBatch.Draw(pixel,
                    new Rectangle((int)conveyor.Position.X, (int)conveyor.Position.Y, (int)conveyor.Width, (int)conveyor.Height),
                    conveyorColor);
                int arrowX = conveyor.Direction == ConveyorDirection.Right ? (int)(conveyor.Position.X + conveyor.Width - 10) : (int)conveyor.Position.X;
                spriteBatch.Draw(pixel, new Rectangle(arrowX, (int)conveyor.Position.Y - 3, 10, 4), Color.White);
            }

            foreach (var portal in model.CurrentLevel.Portals)
            {
                Color portalColor = Color.Purple;
                float portalPulse = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 4) * 0.3f + 0.7f;
                spriteBatch.Draw(pixel,
                    new Rectangle((int)portal.Position.X, (int)portal.Position.Y, (int)portal.Width, (int)portal.Height),
                    new Color((int)(portalColor.R * portalPulse), (int)(portalColor.G * portalPulse), (int)(portalColor.B * portalPulse)));
            }

            foreach (var coin in model.CurrentLevel.Coins)
            {
                if (coin.Collected) continue;
                Color coinColor = coin.Type == CoinType.Gold ? Color.Gold : Color.Yellow;
                float coinPulse = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 6) * 2f;
                int size = (int)(15 + coinPulse);
                int offset = (int)(coinPulse / 2);
                spriteBatch.Draw(pixel,
                    new Rectangle((int)coin.Position.X - offset, (int)coin.Position.Y - offset, size, size),
                    coinColor);
            }

            foreach (var enemy in model.CurrentLevel.Enemies)
                if (enemy.Active)
                    DrawEnemy(enemy, gameTime);

            foreach (var spike in model.CurrentLevel.Spikes)
                DrawSpike(spike);

            foreach (var cp in model.CurrentLevel.Checkpoints)
            {
                Color cpColor = cp.Activated ? Color.LightGreen : Color.DarkGray;
                spriteBatch.Draw(pixel, new Rectangle((int)cp.Position.X, (int)cp.Position.Y, 4, 32), Color.Brown);
                spriteBatch.Draw(pixel, new Rectangle((int)cp.Position.X + 4, (int)cp.Position.Y, 20, 16), cpColor);
            }

            if (model.CurrentLevel.Door != null)
            {
                doorOpen = model.CurrentLevel.Door.IsOpen;
                Color doorColor = doorOpen ? Color.Green : Color.Red;
                spriteBatch.Draw(pixel,
                    new Rectangle((int)model.CurrentLevel.Door.Position.X - 2, (int)model.CurrentLevel.Door.Position.Y - 2,
                        (int)model.CurrentLevel.Door.Width + 4, (int)model.CurrentLevel.Door.Height + 4),
                    Color.DarkGray);
                spriteBatch.Draw(pixel,
                    new Rectangle((int)model.CurrentLevel.Door.Position.X, (int)model.CurrentLevel.Door.Position.Y,
                        (int)model.CurrentLevel.Door.Width, (int)model.CurrentLevel.Door.Height),
                    doorColor);
                spriteBatch.Draw(pixel,
                    new Rectangle((int)model.CurrentLevel.Door.Position.X + 16, (int)model.CurrentLevel.Door.Position.Y + 14, 4, 4),
                    Color.Yellow);
            }

            DrawPlayer(gameTime);

            ui.Draw(currentTime, collectedCoins, requiredCoins, currentScore, model.GameOver, model.LevelCompleted, model.GameWon, model.IsPaused, levelName, levelNumber);

            spriteBatch.End();
        }

        private void UpdateAnimation(float deltaTime)
        {
            animationTimer += deltaTime;
            if (animationTimer >= FrameDuration)
            {
                animationTimer = 0f;
                currentFrame = (currentFrame + 1) % 4;
            }
        }

        private void DrawPlayer(GameTime gameTime)
        {
            var player = model.Player;
            int x = (int)playerPosition.X;
            int y = (int)playerPosition.Y;
            int w = (int)player.Width;
            int h = (int)player.Height;

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