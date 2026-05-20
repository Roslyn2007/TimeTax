using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TimeTax.View
{
    public class UIRenderer
    {
        private SpriteBatch spriteBatch;
        private Texture2D pixel;
        private SpriteFont font;

        public UIRenderer(SpriteBatch spriteBatch, Texture2D pixelTexture, SpriteFont font)
        {
            this.spriteBatch = spriteBatch;
            this.pixel = pixelTexture;
            this.font = font;
        }

        public void Draw(float time, int coins, int required, int score, bool gameOver, bool levelComplete, bool gameWon, bool paused, string levelName, int levelNumber)
        {
            // === ШКАЛА ВРЕМЕНИ ===
            int timeBarWidth = MathHelper.Clamp((int)(time * 4), 0, 760);
            Color timeBarColor = time > 30 ? new Color(0, 255, 100) : time > 10 ? Color.Orange : Color.Red;
            
            spriteBatch.Draw(pixel, new Rectangle(10, 10, 760, 28), Color.Black * 0.7f);
            spriteBatch.Draw(pixel, new Rectangle(12, 12, timeBarWidth, 24), timeBarColor);
            string timeText = $"Time: {time:F1}s / {GetMaxTime(levelNumber)}s";
            spriteBatch.DrawString(font, timeText, new Vector2(16, 14), Color.White);

            // === ИНФО УРОВНЯ ===
            spriteBatch.DrawString(font, $"Level {levelNumber}: {levelName}", new Vector2(10, 44), Color.LightBlue);

            // === МОНЕТЫ — КРУПНО И ВИДНО ===
            Color coinBgColor = coins >= required ? new Color(0, 100, 0) : new Color(100, 80, 0);
            spriteBatch.Draw(pixel, new Rectangle(10, 66, 200, 26), coinBgColor);
            string coinText = $"Coins: {coins} / {required}";
            Color coinTextColor = coins >= required ? Color.LightGreen : Color.Gold;
            spriteBatch.DrawString(font, coinText, new Vector2(14, 68), coinTextColor);
            
            // Полоска прогресса
            int coinBarWidth = required > 0 ? (int)((float)coins / required * 196) : 0;
            coinBarWidth = MathHelper.Clamp(coinBarWidth, 0, 196);
            spriteBatch.Draw(pixel, new Rectangle(12, 88, coinBarWidth, 4), coinTextColor);

            // === СЧЁТ ===
            spriteBatch.DrawString(font, $"Score: {score}", new Vector2(10, 96), Color.LightGreen);

            // === ПОДСКАЗКА ===
            spriteBatch.DrawString(font, "WASD/Arrows: Move | Space: Jump | ESC: Pause", new Vector2(10, 120), Color.Gray, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);

            // === ПАУЗА ===
            if (paused)
            {
                spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 480), new Color(0, 0, 0, 180));
                spriteBatch.DrawString(font, "PAUSED", new Vector2(330, 200), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, "Press ESC to resume", new Vector2(300, 260), Color.Gray);
            }

            // === GAME OVER ===
            if (gameOver)
            {
                spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 480), new Color(0, 0, 0, 200));
                spriteBatch.DrawString(font, "GAME OVER", new Vector2(280, 180), Color.Red, 0f, Vector2.Zero, 2.5f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, "Time ran out!", new Vector2(320, 240), Color.Orange);
                spriteBatch.DrawString(font, "Press ENTER to restart", new Vector2(270, 290), Color.White);
            }
            // === LEVEL COMPLETE ===
            else if (levelComplete && !gameWon)
            {
                spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 480), new Color(0, 0, 0, 180));
                spriteBatch.DrawString(font, "LEVEL COMPLETE!", new Vector2(240, 180), Color.Gold, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, $"Coins: {coins}/{required}", new Vector2(320, 240), Color.White);
                spriteBatch.DrawString(font, "Press ENTER to continue", new Vector2(260, 290), Color.White);
            }
            // === GAME WON ===
            else if (gameWon)
            {
                spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 480), new Color(0, 0, 0, 200));
                spriteBatch.DrawString(font, "YOU ESCAPED!", new Vector2(250, 160), Color.Gold, 0f, Vector2.Zero, 2.5f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, $"Final Score: {score}", new Vector2(300, 230), Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, "Press ENTER for menu", new Vector2(280, 290), Color.Gray);
            }
        }

        private int GetMaxTime(int level)
        {
            return level switch
            {
                1 => 90,
                2 => 80,
                3 => 75,
                4 => 70,
                5 => 65,
                _ => 90
            };
        }
    }
}