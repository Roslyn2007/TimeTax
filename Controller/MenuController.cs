using Microsoft.Xna.Framework.Input;
using TimeTax.View;

namespace TimeTax.Controller
{
    public class MenuController
    {
        private MenuView menuView;
        private KeyboardState previousKeyboard;

        public MenuController(MenuView menuView)
        {
            this.menuView = menuView;
        }

        public void Update(float deltaTime)
        {
            KeyboardState currentKeyboard = Keyboard.GetState();

            bool upPressed = (currentKeyboard.IsKeyDown(Keys.Up) || currentKeyboard.IsKeyDown(Keys.W)) 
                && !previousKeyboard.IsKeyDown(Keys.Up) && !previousKeyboard.IsKeyDown(Keys.W);
            bool downPressed = (currentKeyboard.IsKeyDown(Keys.Down) || currentKeyboard.IsKeyDown(Keys.S)) 
                && !previousKeyboard.IsKeyDown(Keys.Down) && !previousKeyboard.IsKeyDown(Keys.S);
            bool enterPressed = currentKeyboard.IsKeyDown(Keys.Enter) && !previousKeyboard.IsKeyDown(Keys.Enter);

            if (upPressed)
                menuView.SelectPrevious();
            if (downPressed)
                menuView.SelectNext();
            if (enterPressed)
                menuView.ActivateSelected();

            previousKeyboard = currentKeyboard;
        }
    }
}