using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TimeTax.Model;
using TimeTax.View;
using TimeTax.Controller;

namespace TimeTax
{
    public enum GameState
    {
        Menu,
        Playing,
        GameOver,
        Victory
    }

    public class TimeTaxGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont font;

        private GameModel gameModel;
        private GameController gameController;
        private GameView gameView;
        private MenuView menuView;
        private MenuController menuController;

        private GameState currentState = GameState.Menu;
        private Texture2D pixel;

        public TimeTaxGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            font = Content.Load<SpriteFont>("Font");

            menuView = new MenuView(GraphicsDevice, spriteBatch, pixel, font);
            menuController = new MenuController(menuView);

            menuView.StartGameRequested += () =>
            {
                StartGame();
                currentState = GameState.Playing;
            };
            menuView.QuitRequested += () => Exit();
        }

        private void StartGame()
        {
            gameModel = new GameModel();
            gameModel.StartNewGame();
            gameController = new GameController(gameModel);
            gameView = new GameView(GraphicsDevice, spriteBatch, gameModel, pixel, font);

            gameModel.GameLost += () => currentState = GameState.GameOver;
            gameModel.GameWonEvent += () => currentState = GameState.Victory;
        }

        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (currentState)
            {
                case GameState.Menu:
                    menuController.Update(deltaTime);
                    break;
                case GameState.Playing:
                    gameController?.Update(deltaTime);
                    break;
                case GameState.GameOver:
                    HandleGameOverInput();
                    break;
                case GameState.Victory:
                    HandleVictoryInput();
                    break;
            }

            base.Update(gameTime);
        }

        private void HandleGameOverInput()
        {
            KeyboardState kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.Enter))
            {
                currentState = GameState.Menu;
            }
        }

        private void HandleVictoryInput()
        {
            KeyboardState kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.Enter))
            {
                currentState = GameState.Menu;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            switch (currentState)
            {
                case GameState.Menu:
                    menuView.Draw(gameTime);
                    break;
                case GameState.Playing:
                    gameView?.Draw(gameTime);
                    break;
                case GameState.GameOver:
                    DrawGameOver();
                    break;
                case GameState.Victory:
                    DrawVictory();
                    break;
            }

            base.Draw(gameTime);
        }

        private void DrawGameOver()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 480), new Color(0, 0, 0, 200));
            spriteBatch.Draw(pixel, new Rectangle(250, 150, 300, 180), Color.DarkRed);
            spriteBatch.DrawString(font, "GAME OVER", new Microsoft.Xna.Framework.Vector2(320, 180), Color.White);
            spriteBatch.DrawString(font, "Press ENTER", new Microsoft.Xna.Framework.Vector2(315, 240), Color.Yellow);
            spriteBatch.End();
        }

        private void DrawVictory()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(pixel, new Rectangle(0, 0, 800, 480), new Color(0, 0, 0, 200));
            spriteBatch.Draw(pixel, new Rectangle(250, 150, 300, 180), Color.Gold);
            spriteBatch.DrawString(font, "YOU WON!", new Microsoft.Xna.Framework.Vector2(325, 180), Color.Black);
            spriteBatch.DrawString(font, "Press ENTER", new Microsoft.Xna.Framework.Vector2(315, 240), Color.Black);
            spriteBatch.End();
        }
    }
}