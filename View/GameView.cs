using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimeTax.Model;
using System;

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

        private GameModel model;

        public GameView(GraphicsDevice graphicsDevice, SpriteBatch sharedSpriteBatch, GameModel model, Texture2D sharedPixel, SpriteFont font)
        {
            this.spriteBatch = sharedSpriteBatch;
            this.model = model;
            this.pixel = sharedPixel;
            this.font = font;
            ui = new UIRenderer(sharedSpriteBatch, pixel, font);

            model.PlayerMoved += pos => playerPosition = new Microsoft.Xna.Framework.Vector2(pos.X, pos.Y);
            model.Time.TimeChanged += t => currentTime = t;
            model.CoinsChanged += c => collectedCoins = c;
            model.Time.ScreenEffectChanged += eff => screenEffect = eff;
            model.ScoreChanged += s => currentScore = s;
            model.LevelStarted += num =>
            {
                levelNumber = num;
                levelName = model.CurrentLevel?.Name ?? "";
                doorOpen = false;
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

            spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 480), bgColor);

            foreach (var platform in model.CurrentLevel.Platforms)
                spriteBatch.Draw(pixel,
                    new Rectangle((int)platform.Position.X, (int)platform.Position.Y, (int)platform.Width, (int)platform.Height),
                    Color.Gray);

            foreach (var conveyor in model.CurrentLevel.Conveyors)
            {
                Color conveyorColor = conveyor.Direction == TimeTax.Model.Entities.ConveyorDirection.Right ? Color.Cyan : Color.LightBlue;
                spriteBatch.Draw(pixel,
                    new Rectangle((int)conveyor.Position.X, (int)conveyor.Position.Y, (int)conveyor.Width, (int)conveyor.Height),
                    conveyorColor);
                int arrowX = conveyor.Direction == TimeTax.Model.Entities.ConveyorDirection.Right ? (int)(conveyor.Position.X + conveyor.Width - 10) : (int)conveyor.Position.X;
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
                Color coinColor = coin.Type == TimeTax.Model.Entities.CoinType.Gold ? Color.Gold : Color.Yellow;
                spriteBatch.Draw(pixel,
                    new Rectangle((int)coin.Position.X, (int)coin.Position.Y, (int)coin.Width, (int)coin.Height),
                    coinColor);
            }

            foreach (var enemy in model.CurrentLevel.Enemies)
                if (enemy.Active)
                    spriteBatch.Draw(pixel,
                        new Rectangle((int)enemy.Position.X, (int)enemy.Position.Y, (int)enemy.Width, (int)enemy.Height),
                        Color.Red);

            foreach (var spike in model.CurrentLevel.Spikes)
                spriteBatch.Draw(pixel,
                    new Rectangle((int)spike.Position.X, (int)spike.Position.Y, (int)spike.Width, (int)spike.Height),
                    Color.DarkRed);

            foreach (var cp in model.CurrentLevel.Checkpoints)
            {
                Color cpColor = cp.Activated ? Color.LightGreen : Color.DarkGray;
                spriteBatch.Draw(pixel,
                    new Rectangle((int)cp.Position.X, (int)cp.Position.Y, (int)cp.Width, (int)cp.Height),
                    cpColor);
            }

            if (model.CurrentLevel.Door != null)
            {
                doorOpen = model.CurrentLevel.Door.IsOpen;
                Color doorColor = doorOpen ? Color.Green : Color.Red;
                spriteBatch.Draw(pixel,
                    new Rectangle((int)model.CurrentLevel.Door.Position.X, (int)model.CurrentLevel.Door.Position.Y,
                        (int)model.CurrentLevel.Door.Width, (int)model.CurrentLevel.Door.Height),
                    doorColor);
            }

            spriteBatch.Draw(pixel,
                new Rectangle((int)playerPosition.X, (int)playerPosition.Y,
                    (int)model.Player.Width, (int)model.Player.Height),
                Color.LimeGreen);

            ui.Draw(currentTime, collectedCoins, requiredCoins, currentScore, model.GameOver, model.LevelCompleted, model.GameWon, model.IsPaused, levelName, levelNumber);

            spriteBatch.End();
        }
    }
}