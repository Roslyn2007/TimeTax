using Microsoft.Xna.Framework.Input;
using TimeTax.Model;

namespace TimeTax.Controller
{
    public class MenuController
    {
        private MenuModel model;
        private KeyboardState previousKeyboard;

        public MenuController(MenuModel model)
        {
            this.model = model;
        }

        public void Update(float _)
        {
            KeyboardState currentKeyboard = Keyboard.GetState();

            bool upPressed = (currentKeyboard.IsKeyDown(Keys.Up) || currentKeyboard.IsKeyDown(Keys.W))
                && !previousKeyboard.IsKeyDown(Keys.Up) && !previousKeyboard.IsKeyDown(Keys.W);
            bool downPressed = (currentKeyboard.IsKeyDown(Keys.Down) || currentKeyboard.IsKeyDown(Keys.S))
                && !previousKeyboard.IsKeyDown(Keys.Down) && !previousKeyboard.IsKeyDown(Keys.S);
            bool enterPressed = currentKeyboard.IsKeyDown(Keys.Enter) && !previousKeyboard.IsKeyDown(Keys.Enter);
            bool escapePressed = currentKeyboard.IsKeyDown(Keys.Escape) && !previousKeyboard.IsKeyDown(Keys.Escape);

            if (model.IsInOptions)
            {
                if (enterPressed)
                    model.ActivateSelected();
                if (escapePressed)
                    model.GoBack();
            }
            else
            {
                if (upPressed)
                    model.SelectPrevious();
                if (downPressed)
                    model.SelectNext();
                if (enterPressed)
                    model.ActivateSelected();
            }

            previousKeyboard = currentKeyboard;
        }
    }
}