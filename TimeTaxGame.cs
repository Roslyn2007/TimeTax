using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TimeTax.Model;
using TimeTax.View;
using TimeTax.Controller;

namespace TimeTax
{
    public class TimeTaxGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch = null!;
        private SpriteFont font = null!;
        private GameModel gameModel = null!;
        private GameController gameController = null!;
        private GameView gameView = null!;
        private MenuView menuView = null!;
        private MenuController menuController = null!;
        private Texture2D pixel = null!;
        private Texture2D? backgroundTexture;
        private AudioManager? audio;

        private GameState currentState = GameState.Menu;

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

            try
            {
                string bgPath = System.IO.Path.Combine(Content.RootDirectory, "bg.png");
                if (System.IO.File.Exists(bgPath))
                    using (var stream = System.IO.File.OpenRead(bgPath))
                        backgroundTexture = Texture2D.FromStream(GraphicsDevice, stream);
            }
            catch { backgroundTexture = null; }

            font = Content.Load<SpriteFont>("Font");

            audio = new AudioManager();
            try { audio.LoadContent(Content, GraphicsDevice); }
            catch { }

            var menuModel = new MenuModel();
            menuController = new MenuController(menuModel);
            menuView = new MenuView(GraphicsDevice, spriteBatch, pixel, font, menuModel, audio);

            menuModel.StartGameRequested += () =>
            {
                StartGame();
                currentState = GameState.Playing;
            };
            menuModel.QuitRequested += () => Exit();
            menuModel.SoundToggleRequested += () =>
            {
                audio?.ToggleSound();
            };
        }

        private void StartGame()
        {
            gameModel = new GameModel();

            if (audio != null)
            {
                gameModel.CoinsChanged += _ => audio.PlayCoin();
                gameModel.Jumped += () => audio.PlayJump();
                gameModel.DamageTaken += () => audio.PlayHurt();
                gameModel.CheckpointActivated += () => audio.PlayCheckpoint();
                gameModel.PortalUsed += () => audio.PlayPortal();
                gameModel.GameWonEvent += () => audio.PlayVictoryMusic();
                gameModel.GameLost += () => audio.PlayGameOverMusic();
                gameModel.LevelStarted += (_, _) => audio.PlayGameMusic();
                gameModel.LevelCompletedEvent += () => audio.PlayLevelCompleteMusic();
            }

            gameController = new GameController(gameModel);
            gameView = new GameView(GraphicsDevice, spriteBatch, gameModel, pixel, font, backgroundTexture);

            gameModel.GameLost += () => currentState = GameState.GameOver;
            gameModel.GameWonEvent += () => currentState = GameState.Victory;

            gameModel.PauseStateChanged += paused =>
            {
                if (paused)
                {
                    audio?.PauseMusic();
                    gameView.EnterPauseMenu();
                }
                else
                {
                    audio?.PlayGameMusic();
                    gameView.ExitPauseMenu();
                }
            };

            gameModel.StartNewGame();
        }

        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (currentState)
            {
                case GameState.Menu:
                    menuController.Update(deltaTime);
                    audio?.PlayMenuMusic();
                    break;

                case GameState.Playing:
                    HandlePlayingInput(deltaTime);
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

        private void HandlePlayingInput(float deltaTime)
        {
            KeyboardState currentKeyboard = Keyboard.GetState();
            KeyboardState prevKb = previousKeyboard;

            if (gameModel.IsPaused)
            {
                bool upPressed = (currentKeyboard.IsKeyDown(Keys.Up) || currentKeyboard.IsKeyDown(Keys.W))
                    && !prevKb.IsKeyDown(Keys.Up) && !prevKb.IsKeyDown(Keys.W);
                bool downPressed = (currentKeyboard.IsKeyDown(Keys.Down) || currentKeyboard.IsKeyDown(Keys.S))
                    && !prevKb.IsKeyDown(Keys.Down) && !prevKb.IsKeyDown(Keys.S);
                bool enterPressed = currentKeyboard.IsKeyDown(Keys.Enter) && !prevKb.IsKeyDown(Keys.Enter);
                bool escapePressed = currentKeyboard.IsKeyDown(Keys.Escape) && !prevKb.IsKeyDown(Keys.Escape);

                if (upPressed)
                    gameView.PauseMenuSelectPrevious();
                if (downPressed)
                    gameView.PauseMenuSelectNext();
                if (enterPressed)
                {
                    int result = gameView.PauseMenuActivateSelected(audio?.SoundEnabled ?? true);
                    if (result == 0)
                    {
                        gameModel.TogglePause();
                    }
                    else if (result == 1)
                    {
                        audio?.ToggleSound();
                        gameView.UpdateSoundText(audio?.SoundEnabled ?? true);
                    }
                    else if (result == 2)
                    {
                        audio?.StopMusic();
                        gameModel.TogglePause();
                        currentState = GameState.Menu;
                    }
                }
                if (escapePressed)
                {
                    gameModel.TogglePause();
                }

                previousKeyboard = currentKeyboard;
                return;
            }

            gameController?.Update(deltaTime);

            previousKeyboard = currentKeyboard;
        }

        private KeyboardState previousKeyboard;

        private void HandleGameOverInput()
        {
            KeyboardState kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.Enter) && !previousKeyboard.IsKeyDown(Keys.Enter))
            {
                audio?.StopMusic();
                currentState = GameState.Menu;
            }
            previousKeyboard = kb;
        }

        private void HandleVictoryInput()
        {
            KeyboardState kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.Enter) && !previousKeyboard.IsKeyDown(Keys.Enter))
            {
                audio?.StopMusic();
                currentState = GameState.Menu;
            }
            previousKeyboard = kb;
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
                    gameView?.Draw(gameTime);
                    break;
                case GameState.Victory:
                    gameView?.Draw(gameTime);
                    break;
            }

            base.Draw(gameTime);
        }
    }
}