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
            // Шкала времени
            int timeBarWidth = MathHelper.Clamp((int)(time * 4), 0, 760);
            Color timeBarColor = time > 30 ? new Color(0, 255, 100) : time > 10 ? Color.Orange : Color.Red;
            spriteBatch.Draw(pixel, new Rectangle(10, 10, 760, 24), Color.Black * 0.5f);
            spriteBatch.Draw(pixel, new Rectangle(12, 12, timeBarWidth, 20), timeBarColor);
            spriteBatch.DrawString(font, $"Time: {time:F1}s", new Vector2(15, 13), Color.White);

            // Уровень
            spriteBatch.DrawString(font, $"Level {levelNumber}: {levelName}", new Vector2(10, 40), Color.White);

            // Монеты
            Color coinTextColor = coins >= required ? Color.Green : Color.Gold;
            spriteBatch.DrawString(font, $"Coins: {coins}/{required}", new Vector2(10, 60), coinTextColor);

            // Счёт
            spriteBatch.DrawString(font, $"Score: {score}", new Vector2(10, 80), Color.LightGreen);

            // Пауза
            if (paused)
            {
                spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 480), new Color(0, 0, 0, 160));
                spriteBatch.DrawString(font, "PAUSED", new Vector2(350, 200), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, "Press ESC to resume", new Vector2(310, 250), Color.Gray);
            }

            // Game Over
            if (gameOver)
            {
                spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 480), new Color(0, 0, 0, 180));
                spriteBatch.DrawString(font, "GAME OVER", new Vector2(300, 180), Color.Red, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, "Press ENTER to restart", new Vector2(280, 240), Color.White);
            }
            // Level Complete
            else if (levelComplete && !gameWon)
            {
                spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 480), new Color(0, 0, 0, 180));
                spriteBatch.DrawString(font, "LEVEL COMPLETE!", new Vector2(260, 180), Color.Gold, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, "Press ENTER to continue", new Vector2(270, 240), Color.White);
            }
            // Game Won
            else if (gameWon)
            {
                spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 480), new Color(0, 0, 0, 180));
                spriteBatch.DrawString(font, "YOU ESCAPED!", new Vector2(270, 180), Color.Gold, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, $"Final Score: {score}", new Vector2(300, 240), Color.White);
                spriteBatch.DrawString(font, "Press ENTER for menu", new Vector2(280, 280), Color.Gray);
            }
        }
    }
}