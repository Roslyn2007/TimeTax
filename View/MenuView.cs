using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TimeTax.View
{
    public class MenuView
    {
        private SpriteBatch spriteBatch;
        private Texture2D pixel;
        private SpriteFont font;
        private GraphicsDevice graphicsDevice;

        public event Action? StartGameRequested;
        public event Action? QuitRequested;

        private int selectedOption = 0;
        private string[] options = { "START GAME", "QUIT" };

        private readonly Color TitleColor = new Color(180, 30, 30);
        private readonly Color BgColor = new Color(20, 20, 40);

        public MenuView(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Texture2D sharedPixel, SpriteFont font)
        {
            this.graphicsDevice = graphicsDevice;
            this.spriteBatch = spriteBatch;
            this.pixel = sharedPixel;
            this.font = font;
        }

        public void SelectNext() => selectedOption = (selectedOption + 1) % options.Length;
        public void SelectPrevious() => selectedOption = (selectedOption - 1 + options.Length) % options.Length;

        public void ActivateSelected()
        {
            if (selectedOption == 0)
                StartGameRequested?.Invoke();
            else if (selectedOption == 1)
                QuitRequested?.Invoke();
        }

        public void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            // Фон
            spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 480), BgColor);

            // Заголовок
            spriteBatch.Draw(pixel, new Rectangle(150, 60, 500, 80), TitleColor);
            spriteBatch.DrawString(font, "TIME TAX", new Vector2(320, 85), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);

            // START
            Color startColor = selectedOption == 0 ? Color.Yellow : Color.White;
            spriteBatch.Draw(pixel, new Rectangle(250, 200, 300, 50), selectedOption == 0 ? Color.Green * 0.5f : Color.Gray * 0.3f);
            spriteBatch.DrawString(font, "START GAME", new Vector2(330, 215), startColor);

            // QUIT
            Color quitColor = selectedOption == 1 ? Color.Yellow : Color.White;
            spriteBatch.Draw(pixel, new Rectangle(250, 300, 300, 50), selectedOption == 1 ? Color.Red * 0.5f : Color.Gray * 0.3f);
            spriteBatch.DrawString(font, "QUIT", new Vector2(360, 315), quitColor);

            // Подсказка
            spriteBatch.DrawString(font, "UP/DOWN to select, ENTER to confirm", new Vector2(250, 420), Color.Gray);

            spriteBatch.End();
        }
    }
}