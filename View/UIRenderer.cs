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
                DrawCenteredOverlay(new Color(0, 0, 0, 200));
                DrawCenteredTitle("GAME OVER", 160, Color.Red, 2.5f);
                DrawCenteredText("Time ran out!", 230, Color.Orange, 1.0f);
                DrawCenteredText("Press ENTER to restart", 280, Color.White, 1.0f);
            }
            else if (levelComplete && !gameWon)
            {
                DrawCenteredOverlay(new Color(0, 0, 0, 180));
                DrawCenteredTitle("LEVEL COMPLETE!", 140, Color.Gold, 2.0f);
                DrawCenteredText($"Level {levelNumber} cleared!", 210, Color.LightBlue, 1.0f);
                DrawCenteredText($"Coins: {coins}/{required}", 250, Color.White, 1.0f);
                DrawCenteredText($"Score: {score}", 290, Color.LightGreen, 1.0f);
                DrawCenteredText("Press ENTER to continue", 340, Color.White, 1.0f);
            }
            else if (gameWon)
            {
                DrawCenteredOverlay(new Color(0, 0, 0, 200));
                DrawCenteredTitle("YOU ESCAPED!", 120, Color.Gold, 2.5f);
                DrawCenteredText("All levels completed!", 200, Color.LightBlue, 1.0f);
                DrawCenteredText($"Final Score: {score}", 250, Color.White, 1.5f);
                DrawCenteredText("Press ENTER for menu", 320, Color.Gray, 1.0f);
            }
        }

        private void DrawCenteredOverlay(Color color)
        {
            spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 480), color);
        }

        private void DrawCenteredTitle(string text, int y, Color color, float scale)
        {
            Vector2 size = font.MeasureString(text) * scale;
            float x = (800 - size.X) / 2f;
            spriteBatch.DrawString(font, text, new Vector2(x, y), color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
        }

        private void DrawCenteredText(string text, int y, Color color, float scale)
        {
            Vector2 size = font.MeasureString(text) * scale;
            float x = (800 - size.X) / 2f;
            spriteBatch.DrawString(font, text, new Vector2(x, y), color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
        }

        private int GetMaxTime(int level)
        {
            return Math.Max(50, 95 - level * 8);
        }
    }
}