using Microsoft.Xna.Framework.Input;
using TimeTax.Model;

namespace TimeTax.Controller
{
    public class GameController
    {
        private GameModel model;
        private KeyboardState previousKeyboard;

        public GameController(GameModel model)
        {
            this.model = model;
        }

        public void Update(float deltaTime)
        {
            KeyboardState currentKeyboard = Keyboard.GetState();

            bool left = currentKeyboard.IsKeyDown(Keys.Left) || currentKeyboard.IsKeyDown(Keys.A);
            bool right = currentKeyboard.IsKeyDown(Keys.Right) || currentKeyboard.IsKeyDown(Keys.D);
            bool jumpPressed = currentKeyboard.IsKeyDown(Keys.Space) && !previousKeyboard.IsKeyDown(Keys.Space);

            if (left && !right)
                model.MoveLeft();
            else if (right && !left)
                model.MoveRight();
            else
                model.StopHorizontal();

            if (jumpPressed)
                model.Jump();

            if (currentKeyboard.IsKeyDown(Keys.R) && !previousKeyboard.IsKeyDown(Keys.R))
                model.StartLevel(model.CurrentLevelNumber);

            if (currentKeyboard.IsKeyDown(Keys.Escape) && !previousKeyboard.IsKeyDown(Keys.Escape))
                model.TogglePause();

            if (model.LevelCompleted && !model.GameWon)
            {
                if ((currentKeyboard.IsKeyDown(Keys.Enter) && !previousKeyboard.IsKeyDown(Keys.Enter)) ||
                    (currentKeyboard.IsKeyDown(Keys.Space) && !previousKeyboard.IsKeyDown(Keys.Space)))
                {
                    model.NextLevel();
                }
            }

            model.Update(deltaTime);

            previousKeyboard = currentKeyboard;
        }
    }
}