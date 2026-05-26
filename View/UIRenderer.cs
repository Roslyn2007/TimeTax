using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TimeTax.View
{
    public class UIRenderer
    {
        private SpriteBatch spriteBatch;
        private Texture2D pixel;
        private SpriteFont font;

        private int selectedPauseOption = 0;
        private string[] pauseOptions = { "RESUME", "SOUND: ON", "MAIN MENU" };
        private bool inPauseMenu = false;

        public UIRenderer(SpriteBatch spriteBatch, Texture2D pixelTexture, SpriteFont font)
        {
            this.spriteBatch = spriteBatch;
            this.pixel = pixelTexture;
            this.font = font;
        }

        public bool IsInPauseMenu => inPauseMenu;

        public void EnterPauseMenu()
        {
            inPauseMenu = true;
            selectedPauseOption = 0;
        }

        public void ExitPauseMenu()
        {
            inPauseMenu = false;
        }

        public void SelectNext()
        {
            if (!inPauseMenu) return;
            selectedPauseOption = (selectedPauseOption + 1) % pauseOptions.Length;
        }

        public void SelectPrevious()
        {
            if (!inPauseMenu) return;
            selectedPauseOption = (selectedPauseOption - 1 + pauseOptions.Length) % pauseOptions.Length;
        }

        public int ActivateSelected(bool soundEnabled)
        {
            if (!inPauseMenu) return -1;

            if (selectedPauseOption == 1)
            {
                return 1;
            }
            else if (selectedPauseOption == 2)
            {
                return 2;
            }

            return 0;
        }

        public void UpdateSoundText(bool soundEnabled)
        {
            pauseOptions[1] = soundEnabled ? "SOUND: ON" : "SOUND: OFF";
        }

        public static (Color bg, Color text) GetButtonColors(int index, bool selected)
        {
            if (index == 0)
                return (selected ? new Color(0, 150, 0) : new Color(0, 80, 0), selected ? Color.LightGreen : Color.White);
            if (index == 2)
                return (selected ? new Color(180, 0, 0) : new Color(100, 0, 0), selected ? Color.OrangeRed : Color.White);
            return (selected ? new Color(100, 100, 150) : new Color(60, 60, 80), selected ? Color.Yellow : Color.White);
        }

        public void Draw(float time, int coins, int required, int score, bool gameOver, bool levelComplete, bool gameWon, bool paused, string levelName, int levelNumber)
        {
            int timeBarWidth = MathHelper.Clamp((int)(time * 4), 0, 760);
            Color timeBarColor = time > 30 ? new Color(0, 255, 100) : time > 10 ? Color.Orange : Color.Red;

            spriteBatch.Draw(pixel, new Rectangle(10, 10, 760, 28), Color.Black * 0.7f);
            spriteBatch.Draw(pixel, new Rectangle(12, 12, timeBarWidth, 24), timeBarColor);
            string timeText = $"Time: {time:F1}s / {GetMaxTime(levelNumber)}s";
            spriteBatch.DrawString(font, timeText, new Vector2(16, 14), Color.White);

            spriteBatch.DrawString(font, $"Level {levelNumber}: {levelName}", new Vector2(10, 44), Color.LightBlue);

            Color coinBgColor = coins >= required ? new Color(0, 100, 0) : new Color(100, 80, 0);
            spriteBatch.Draw(pixel, new Rectangle(10, 66, 200, 26), coinBgColor);
            string coinText = $"Coins: {coins} / {required}";
            Color coinTextColor = coins >= required ? Color.LightGreen : Color.Gold;
            spriteBatch.DrawString(font, coinText, new Vector2(14, 68), coinTextColor);

            int coinBarWidth = required > 0 ? (int)((float)coins / required * 196) : 0;
            coinBarWidth = MathHelper.Clamp(coinBarWidth, 0, 196);
            spriteBatch.Draw(pixel, new Rectangle(12, 88, coinBarWidth, 4), coinTextColor);

            spriteBatch.DrawString(font, $"Score: {score}", new Vector2(10, 96), Color.LightGreen);

            spriteBatch.DrawString(font, "WASD/Arrows: Move | Space: Jump | ESC: Pause", new Vector2(10, 120), Color.Gray, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);

            if (paused)
            {
                spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 480), new Color(0, 0, 0, 180));

                if (inPauseMenu)
                {
                    spriteBatch.DrawString(font, "PAUSED", new Vector2(330, 120), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);

                    for (int i = 0; i < pauseOptions.Length; i++)
                    {
                        int y = 200 + i * 70;
                        bool isSelected = selectedPauseOption == i;
                        var (bgColor, textColor) = GetButtonColors(i, isSelected);

                        spriteBatch.Draw(pixel, new Rectangle(250, y, 300, 50), bgColor);
                        spriteBatch.DrawString(font, pauseOptions[i], new Vector2(330, y + 15), textColor);
                    }

                    spriteBatch.DrawString(font, "UP/DOWN to select, ENTER to confirm", new Vector2(250, 440), Color.Gray);
                }
                else
                {
                    spriteBatch.DrawString(font, "PAUSED", new Vector2(330, 200), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, "Press ESC to resume", new Vector2(300, 260), Color.Gray);
                }
            }

            if (gameOver)
            {
                spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 480), new Color(0, 0, 0, 200));
                spriteBatch.DrawString(font, "GAME OVER", new Vector2(280, 180), Color.Red, 0f, Vector2.Zero, 2.5f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, "Time ran out!", new Vector2(320, 240), Color.Orange);
                spriteBatch.DrawString(font, "Press ENTER to restart", new Vector2(270, 290), Color.White);
            }
            else if (levelComplete && !gameWon)
            {
                spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 480), new Color(0, 0, 0, 180));
                spriteBatch.DrawString(font, "LEVEL COMPLETE!", new Vector2(240, 180), Color.Gold, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, $"Coins: {coins}/{required}", new Vector2(320, 240), Color.White);
                spriteBatch.DrawString(font, "Press ENTER to continue", new Vector2(260, 290), Color.White);
            }
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
            return Math.Max(50, 95 - level * 8);
        }
    }
}