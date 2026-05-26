using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TimeTax.Model;

namespace TimeTax.View
{
    public class MenuView
    {
        private SpriteBatch spriteBatch;
        private Texture2D pixel;
        private SpriteFont font;
        private AudioManager? audio;

        private int selectedOption = 0;
        private bool isInOptions = false;
        private IReadOnlyList<string> options;

        private readonly Color TitleColor = new Color(180, 30, 30);
        private readonly Color BgColor = new Color(20, 20, 40);

        private Texture2D? background;

        public MenuView(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Texture2D sharedPixel, SpriteFont font, MenuModel model, AudioManager? audio = null, Texture2D? background = null)
        {
            this.spriteBatch = spriteBatch;
            this.pixel = sharedPixel;
            this.font = font;
            this.audio = audio;
            this.background = background;

            options = model.Options;
            model.SelectedOptionChanged += idx => selectedOption = idx;
            model.OptionsStateChanged += state => isInOptions = state;
        }

        public void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            if (background != null)
            {
                spriteBatch.Draw(background, new Rectangle(0, 0, 800, 480), Color.White);
                spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 480), BgColor * 0.5f);
            }
            else
            {
                spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 480), BgColor);
            }

            spriteBatch.Draw(pixel, new Rectangle(150, 60, 500, 80), TitleColor);
            spriteBatch.DrawString(font, "TIME TAX", new Microsoft.Xna.Framework.Vector2(320, 85), Color.White, 0f, Microsoft.Xna.Framework.Vector2.Zero, 2f, SpriteEffects.None, 0);

            if (isInOptions)
                DrawOptions();
            else
                DrawMainMenu();

            spriteBatch.End();
        }

        private void DrawMainMenu()
        {
            for (int i = 0; i < options.Count; i++)
            {
                int y = 200 + i * 80;
                bool isSelected = selectedOption == i;
                var (bgColor, textColor) = UIRenderer.GetButtonColors(i, isSelected);

                spriteBatch.Draw(pixel, new Rectangle(250, y, 300, 50), bgColor);
                spriteBatch.DrawString(font, options[i], new Microsoft.Xna.Framework.Vector2(330, y + 15), textColor);
            }

            spriteBatch.DrawString(font, "UP/DOWN to select, ENTER to confirm", new Microsoft.Xna.Framework.Vector2(250, 420), Color.Gray);
        }

        private void DrawOptions()
        {
            spriteBatch.DrawString(font, "OPTIONS", new Microsoft.Xna.Framework.Vector2(340, 150), Color.White, 0f, Microsoft.Xna.Framework.Vector2.Zero, 1.5f, SpriteEffects.None, 0);

            string soundStatus = audio?.SoundEnabled == true ? "ON" : "OFF";
            spriteBatch.Draw(pixel, new Rectangle(250, 250, 300, 50), new Color(60, 60, 80));
            spriteBatch.DrawString(font, $"SOUND: {soundStatus}", new Microsoft.Xna.Framework.Vector2(330, 265), Color.Yellow);

            spriteBatch.DrawString(font, "ENTER to toggle, ESC to back", new Microsoft.Xna.Framework.Vector2(260, 350), Color.Gray);
        }
    }
}