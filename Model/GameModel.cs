using System;
using System.Collections.Generic;
using TimeTax.Model.Entities;
using TimeTax.Model.Interfaces;

namespace TimeTax.Model
{
    public class GameModel
    {
        public Player Player { get; private set; }
        public Level CurrentLevel { get; private set; }
        public TimeManager Time { get; private set; }

        public int CollectedCoins { get; private set; }
        public int TotalCoinsRequired => CurrentLevel?.RequiredCoins ?? 0;
        public bool LevelCompleted { get; private set; }
        public bool GameOver { get; private set; }
        public bool GameWon { get; private set; }
        public int CurrentLevelNumber { get; private set; }
        public int TotalLevels => 5;
        public int Score { get; private set; }
        public bool IsPaused { get; set; }

        public event Action<int> CoinsChanged;
        public event Action LevelFinished;
        public event Action GameLost;
        public event Action<string> PlaySound;
        public event Action<Vector2> PlayerMoved;
        public event Action<int> LevelStarted;
        public event Action GameWonEvent;
        public event Action<int> ScoreChanged;

        private float penaltyCooldown = 0f;
        private float portalCooldown = 0f;

        public void StartNewGame()
        {
            CurrentLevelNumber = 1;
            Score = 0;
            GameWon = false;
            StartLevel(CurrentLevelNumber);
        }

        public void StartLevel(int levelNumber)
        {
            CurrentLevelNumber = levelNumber;
            CurrentLevel = new Level();
            CurrentLevel.LoadLevel(levelNumber);
            Time = new TimeManager(CurrentLevel.StartTime);
            Player = new Player { Position = CurrentLevel.PlayerSpawn };

            CollectedCoins = 0;
            LevelCompleted = false;
            GameOver = false;
            IsPaused = false;
            penaltyCooldown = 0f;
            portalCooldown = 0f;

            CoinsChanged?.Invoke(CollectedCoins);
            PlayerMoved?.Invoke(Player.Position);
            LevelStarted?.Invoke(levelNumber);
            ScoreChanged?.Invoke(Score);
        }

        public void NextLevel()
        {
            int timeBonus = (int)(Time.CurrentTime * 10);
            float multiplier = Time.CurrentTime > 30 ? 3f : Time.CurrentTime > 10 ? 2f : 1f;
            int levelScore = (int)((CollectedCoins * 100 + timeBonus) * multiplier);
            Score += levelScore;
            ScoreChanged?.Invoke(Score);

            if (CurrentLevelNumber >= TotalLevels)
            {
                GameWon = true;
                GameWonEvent?.Invoke();
            }
            else
            {
                StartLevel(CurrentLevelNumber + 1);
            }
        }

        public void Update(float deltaTime)
        {
            if (LevelCompleted || GameOver || GameWon || IsPaused) return;

            foreach (var enemy in CurrentLevel.Enemies)
                enemy.Update(deltaTime);

            Time.Update(deltaTime);
            if (Time.CurrentTime <= 0)
            {
                GameOver = true;
                GameLost?.Invoke();
                return;
            }

            Player.Update(deltaTime, Player.Gravity);
            ApplyConveyorEffect(deltaTime);
            ResolvePlatformCollisions();
            HandlePortals();
            HandleCoinCollection();
            HandleEnemyCollision();
            HandleSpikeCollision();
            HandleCheckpoint();
            CheckExit();

            if (Player.Position.X < 0) Player.Position = new Vector2(0, Player.Position.Y);
            if (Player.Position.X + Player.Width > 800) Player.Position = new Vector2(800 - Player.Width, Player.Position.Y);
            if (Player.Position.Y < -50) Player.Position = new Vector2(Player.Position.X, -50);

            if (Player.Position.Y > 480)
            {
                ApplyPenalty(10f, true);
            }

            if (penaltyCooldown > 0) penaltyCooldown -= deltaTime;
            if (portalCooldown > 0) portalCooldown -= deltaTime;

            PlayerMoved?.Invoke(Player.Position);
        }

        private void ApplyConveyorEffect(float deltaTime)
        {
            foreach (var conveyor in CurrentLevel.Conveyors)
            {
                var cBounds = conveyor.GetBounds();
                var pBounds = Player.GetBounds();

                if (pBounds.right > cBounds.left && pBounds.left < cBounds.right &&
                    pBounds.bottom >= cBounds.top && pBounds.bottom <= cBounds.bottom + 5)
                {
                    float push = conveyor.Direction == ConveyorDirection.Right ? conveyor.Speed : -conveyor.Speed;
                    Player.Position = new Vector2(Player.Position.X + push * deltaTime, Player.Position.Y);
                }
            }
        }

        private void ResolvePlatformCollisions()
        {
            foreach (var platform in CurrentLevel.Platforms)
            {
                var pBounds = platform.GetBounds();
                var plBounds = Player.GetBounds();

                if (plBounds.right > pBounds.left && plBounds.left < pBounds.right &&
                    plBounds.bottom > pBounds.top && plBounds.top < pBounds.bottom)
                {
                    float overlapTop = plBounds.bottom - pBounds.top;
                    float overlapBottom = pBounds.bottom - plBounds.top;
                    float overlapLeft = plBounds.right - pBounds.left;
                    float overlapRight = pBounds.right - plBounds.left;

                    float minOverlap = Math.Min(Math.Min(overlapTop, overlapBottom), Math.Min(overlapLeft, overlapRight));

                    if (minOverlap == overlapTop && Player.Velocity.Y > 0)
                    {
                        Player.Position = new Vector2(Player.Position.X, pBounds.top - Player.Height);
                        Player.Velocity = new Vector2(Player.Velocity.X, 0);
                        Player.IsGrounded = true;
                    }
                    else if (minOverlap == overlapBottom && Player.Velocity.Y < 0)
                    {
                        Player.Position = new Vector2(Player.Position.X, pBounds.bottom);
                        Player.Velocity = new Vector2(Player.Velocity.X, 0);
                    }
                    else if (minOverlap == overlapLeft)
                    {
                        Player.Position = new Vector2(pBounds.left - Player.Width, Player.Position.Y);
                        Player.Velocity = new Vector2(0, Player.Velocity.Y);
                    }
                    else if (minOverlap == overlapRight)
                    {
                        Player.Position = new Vector2(pBounds.right, Player.Position.Y);
                        Player.Velocity = new Vector2(0, Player.Velocity.Y);
                    }
                }
            }
        }

        private void HandlePortals()
        {
            if (portalCooldown > 0) return;

            foreach (var portal in CurrentLevel.Portals)
            {
                if (!portal.Active) continue;
                if (CheckCollision(Player, portal))
                {
                    Player.Position = portal.TargetPosition;
                    Player.Velocity = new Vector2(Player.Velocity.X * 0.5f, 0);
                    portalCooldown = 1f;
                    PlaySound?.Invoke("portal");
                    break;
                }
            }
        }

        private void HandleCoinCollection()
        {
            foreach (var coin in CurrentLevel.Coins)
            {
                if (coin.Collected) continue;
                if (CheckCollision(Player, coin))
                {
                    coin.Collected = true;
                    CollectedCoins++;
                    CoinsChanged?.Invoke(CollectedCoins);

                    if (coin.Type == CoinType.Gold)
                        Time.AddSeconds(10);
                    else
                        Time.AddSeconds(5);

                    PlaySound?.Invoke("coin");
                }
            }
        }

        private void HandleEnemyCollision()
        {
            if (penaltyCooldown > 0) return;

            foreach (var enemy in CurrentLevel.Enemies)
            {
                if (!enemy.Active) continue;
                if (CheckCollision(Player, enemy))
                {
                    ApplyPenalty(5f, false);
                    return;
                }
            }
        }

        private void HandleSpikeCollision()
        {
            if (penaltyCooldown > 0) return;

            foreach (var spike in CurrentLevel.Spikes)
            {
                if (CheckCollision(Player, spike))
                {
                    ApplyPenalty(8f, false);
                    return;
                }
            }
        }

        private void HandleCheckpoint()
        {
            foreach (var cp in CurrentLevel.Checkpoints)
            {
                if (cp.Activated) continue;
                if (CheckCollision(Player, cp))
                {
                    cp.Activated = true;
                    PlaySound?.Invoke("checkpoint");
                }
            }
        }

        private void CheckExit()
        {
            if (CurrentLevel.Door == null) return;
            if (CollectedCoins >= CurrentLevel.RequiredCoins)
            {
                CurrentLevel.Door.IsOpen = true;
                if (CheckCollision(Player, CurrentLevel.Door))
                {
                    LevelCompleted = true;
                    LevelFinished?.Invoke();
                }
            }
        }

        private void ApplyPenalty(float seconds, bool respawn)
        {
            Time.SubtractSeconds(seconds);
            PlaySound?.Invoke("damage");

            if (respawn)
            {
                Vector2 respawnPoint = CurrentLevel.PlayerSpawn;
                foreach (var cp in CurrentLevel.Checkpoints)
                {
                    if (cp.Activated)
                        respawnPoint = cp.Position;
                }
                Player.Position = respawnPoint;
                Player.Velocity = Vector2.Zero;
                Player.IsGrounded = false;
            }

            penaltyCooldown = 0.8f;
        }

        private bool CheckCollision(ICollidable a, ICollidable b)
        {
            var ab = a.GetBounds();
            var bb = b.GetBounds();
            return ab.left < bb.right && ab.right > bb.left && ab.top < bb.bottom && ab.bottom > bb.top;
        }

        public void MoveLeft()
        {
            if (!LevelCompleted && !GameOver && !GameWon && !IsPaused)
                Player.Velocity = new Vector2(-Player.MoveSpeed, Player.Velocity.Y);
        }

        public void MoveRight()
        {
            if (!LevelCompleted && !GameOver && !GameWon && !IsPaused)
                Player.Velocity = new Vector2(Player.MoveSpeed, Player.Velocity.Y);
        }

        public void StopHorizontal()
        {
            Player.Velocity = new Vector2(0, Player.Velocity.Y);
        }

        public void Jump()
        {
            if (Player.IsGrounded && !LevelCompleted && !GameOver && !GameWon && !IsPaused)
            {
                Player.Velocity = new Vector2(Player.Velocity.X, Player.JumpVelocity);
                Player.IsGrounded = false;
            }
        }

        public void TogglePause()
        {
            IsPaused = !IsPaused;
        }
    }
}