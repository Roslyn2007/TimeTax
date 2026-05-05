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

        public event Action<int> CoinsChanged;
        public event Action LevelFinished;
        public event Action GameLost;
        public event Action<string> PlaySound; // "coin", "damage", "heartbeat"
        public event Action<Vector2> PlayerMoved;

        private float penaltyCooldown = 0f; // чтобы не снимать время каждый кадр при касании

        public void StartNewLevel()
        {
            CurrentLevel = new Level();
            CurrentLevel.LoadTestLevel();
            Time = new TimeManager(CurrentLevel.StartTime);
            Player = new Player { Position = CurrentLevel.PlayerSpawn };

            CollectedCoins = 0;
            LevelCompleted = false;
            GameOver = false;

            CoinsChanged?.Invoke(CollectedCoins);
            PlayerMoved?.Invoke(Player.Position);
        }

        public void Update(float deltaTime)
        {
            if (LevelCompleted || GameOver) return;

            // Обновление времени
            Time.Update(deltaTime);
            if (Time.CurrentTime <= 0)
            {
                GameOver = true;
                GameLost?.Invoke();
                return;
            }

            // Физика игрока
            Player.Update(deltaTime, Player.Gravity);

            // Коллизии с платформами (упрощённое разрешение)
            ResolvePlatformCollisions();

            // Сбор предметов и урон
            HandleCoinCollection();
            HandleEnemyCollision();
            HandleSpikeCollision();
            HandleCheckpoint();

            // Проверка выхода
            CheckExit();

            // Ограничение игрока экраном (чтобы не улетел)
            if (Player.Position.X < 0) Player.Position = new Vector2(0, Player.Position.Y);
            if (Player.Position.X + Player.Width > 800) Player.Position = new Vector2(800 - Player.Width, Player.Position.Y);

            // Падение в пропасть (ниже экрана)
            if (Player.Position.Y > 480)
            {
                ApplyPenalty(10f, true);
            }

            if (penaltyCooldown > 0) penaltyCooldown -= deltaTime;

            PlayerMoved?.Invoke(Player.Position);
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
                    // Вертикальное разрешение
                    float overlapTop = plBounds.bottom - pBounds.top;
                    float overlapBottom = pBounds.bottom - plBounds.top;
                    float overlapLeft = plBounds.right - pBounds.left;
                    float overlapRight = pBounds.right - plBounds.left;

                    float minOverlap = Math.Min(Math.Min(overlapTop, overlapBottom), Math.Min(overlapLeft, overlapRight));

                    if (minOverlap == overlapTop && Player.Velocity.Y > 0) // падал на платформу
                    {
                        Player.Position = new Vector2(Player.Position.X, pBounds.top - Player.Height);
                        Player.Velocity = new Vector2(Player.Velocity.X, 0);
                        Player.IsGrounded = true;
                    }
                    else if (minOverlap == overlapBottom && Player.Velocity.Y < 0) // ударился снизу
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
                // Возврат на последний активированный чекпоинт или спавн
                Vector2 respawnPoint = CurrentLevel.PlayerSpawn;
                foreach (var cp in CurrentLevel.Checkpoints)
                {
                    if (cp.Activated)
                        respawnPoint = cp.Position;
                }
                Player.Position = respawnPoint;
                Player.Velocity = Vector2.Zero;
            }

            penaltyCooldown = 0.5f;
        }

        private bool CheckCollision(ICollidable a, ICollidable b)
        {
            var ab = a.GetBounds();
            var bb = b.GetBounds();
            return ab.left < bb.right && ab.right > bb.left && ab.top < bb.bottom && ab.bottom > bb.top;
        }

        // Движение по командам контроллера
        public void MoveLeft()
        {
            if (!LevelCompleted && !GameOver)
                Player.Velocity = new Vector2(-Player.MoveSpeed, Player.Velocity.Y);
        }

        public void MoveRight()
        {
            if (!LevelCompleted && !GameOver)
                Player.Velocity = new Vector2(Player.MoveSpeed, Player.Velocity.Y);
        }

        public void StopHorizontal()
        {
            Player.Velocity = new Vector2(0, Player.Velocity.Y);
        }

        public void Jump()
        {
            if (Player.IsGrounded && !LevelCompleted && !GameOver)
            {
                Player.Velocity = new Vector2(Player.Velocity.X, Player.JumpVelocity);
                Player.IsGrounded = false;
            }
        }
    }
}