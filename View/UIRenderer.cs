using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TimeTax.View
{
    public class UIRenderer
    {
        private SpriteBatch spriteBatch;
        private Texture2D pixel;

        public UIRenderer(SpriteBatch spriteBatch, Texture2D pixelTexture)
        {
            this.spriteBatch = spriteBatch;
            this.pixel = pixelTexture;
        }

        public void Draw(float time, int coins, int required, bool gameOver, bool levelComplete)
        {
            int timeBarWidth = (int)(time * 4); // 4 пикселя на секунду
            spriteBatch.Draw(pixel, new Rectangle(10, 10, timeBarWidth, 20), Color.White);

            if (gameOver)
            {
                // затемнение
                spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 480), new Color(0, 0, 0, 180));
                spriteBatch.Draw(pixel, new Rectangle(300, 200, 200, 60), Color.DarkRed);
            }
            else if (levelComplete)
            {
                spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 480), new Color(0, 0, 0, 180));
                spriteBatch.Draw(pixel, new Rectangle(300, 200, 200, 60), Color.Gold);
            }
        }
    }
}