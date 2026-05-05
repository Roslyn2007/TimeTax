using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimeTax.Model;
using TimeTax.View;
using TimeTax.Controller;

namespace TimeTax
{
    public class TimeTaxGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private GameModel gameModel;
        private GameController gameController;
        private GameView gameView;

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
            gameModel = new GameModel();
            gameModel.StartNewLevel();

            gameController = new GameController(gameModel);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            gameView = new GameView(GraphicsDevice, spriteBatch, gameModel);
        }

        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            gameController.Update(deltaTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            gameView.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}