using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using TimeTax.Model.Entities;
using TimeTax.Model.Collision;
using TimeTax.Model.Pathfinding;

namespace TimeTax.Model
{
    public class GameModel
    {
        public Player Player { get; private set; } = null!;
        public Level CurrentLevel { get; private set; } = null!;
        public TimeManager Time { get; private set; } = null!;
        public CollisionManager CollisionManager { get; private set; } = null!;
        public AStarPathfinder Pathfinder { get; private set; } = null!;

        public int CollectedCoins { get; private set; }
        public int TotalCoinsRequired => CurrentLevel?.RequiredCoins ?? 0;
        public bool LevelCompleted { get; private set; }
        public bool GameOver { get; private set; }
        public bool GameWon { get; private set; }
        public int CurrentLevelNumber { get; private set; }
        public int TotalLevels => 5;
        public int Score { get; private set; }
        public bool IsPaused { get; private set; }

        public void TogglePause()
        {
            IsPaused = !IsPaused;
            PauseStateChanged?.Invoke(IsPaused);
        }

        public event Action<int, Level>? LevelStarted;
        public event Action<Player>? PlayerCreated;
        public event Action<int>? CoinsChanged;
        public event Action? GameLost;
        public event Action<Vector2>? PlayerMoved;
        public event Action? GameWonEvent;
        public event Action<int>? ScoreChanged;
        public event Action<TimeManager>? TimeManagerChanged;
        public event Action<bool>? PauseStateChanged;
        public event Action<List<Enemy>>? EnemiesChanged;
        public event Action? PlayGameMusic;

        public event Action? Jumped;
        public event Action? DamageTaken;
        public event Action? CheckpointActivated;
        public event Action? PortalUsed;
        public event Action? LevelCompletedEvent;

        public event Action<Coin>? CoinCollectedEvent;
        public event Action<Checkpoint>? CheckpointStateChanged;
        public event Action<bool>? DoorStateChanged;
        public event Action? DoorOpened;
        public event Action<FadingPlatform>? FadingPlatformChanged;

        public event Action<string>? BackgroundChanged;

        private float penaltyCooldown = 0f;
        private float portalCooldown = 0f;
        private List<SmartEnemy> smartEnemies = new List<SmartEnemy>();
        private bool playerFellToVoid = false;

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
            TimeManagerChanged?.Invoke(Time);

            Player = new Player { Position = CurrentLevel.PlayerSpawn };
            PlayerCreated?.Invoke(Player);

            CollectedCoins = 0;
            LevelCompleted = false;
            GameOver = false;
            IsPaused = false;
            penaltyCooldown = 0f;
            portalCooldown = 0f;
            playerFellToVoid = false;

            CollisionManager = new CollisionManager(800, 480);
            RebuildQuadTree();

            foreach (var fp in CurrentLevel.FadingPlatforms)
            {
                fp.VisibilityChanged += fpChanged =>
                {
                    RebuildQuadTree();
                    FadingPlatformChanged?.Invoke(fpChanged);
                };
            }

            Pathfinder = new AStarPathfinder(CurrentLevel);

            if (levelNumber > 2)
                ConvertToSmartEnemies();

            foreach (var enemy in CurrentLevel.Enemies)
            {
                enemy.SetPlatforms(CurrentLevel.Platforms, CurrentLevel.FadingPlatforms);
            }

            CoinsChanged?.Invoke(CollectedCoins);
            PlayerMoved?.Invoke(Player.Position);
            LevelStarted?.Invoke(levelNumber, CurrentLevel);
            BackgroundChanged?.Invoke(CurrentLevel.BackgroundFileName);
            EnemiesChanged?.Invoke(CurrentLevel.Enemies);
            ScoreChanged?.Invoke(Score);
            PlayGameMusic?.Invoke();
        }

        private void RebuildQuadTree()
        {
            var staticObjects = new List<ICollidable>();
            staticObjects.AddRange(CurrentLevel.Platforms);
            staticObjects.AddRange(CurrentLevel.FadingPlatforms.Where(fp => fp.IsVisible));
            staticObjects.AddRange(CurrentLevel.Spikes);
            staticObjects.AddRange(CurrentLevel.Conveyors);
            if (CurrentLevel.Door != null) staticObjects.Add(CurrentLevel.Door);

            CollisionManager.RebuildQuadTree(staticObjects);
            Pathfinder?.RebuildWalkableGrid();
        }

        private void ConvertToSmartEnemies()
        {
            smartEnemies.Clear();
            var oldEnemies = new List<Enemy>(CurrentLevel.Enemies);
            CurrentLevel.Enemies.Clear();

            foreach (var old in oldEnemies)
            {
                var smart = new SmartEnemy
                {
                    Position = old.Position,
                    SpawnPosition = old.SpawnPosition,
                    PatrolStartX = old.PatrolStartX,
                    PatrolEndX = old.PatrolEndX,
                    PatrolSpeed = old.PatrolSpeed,
                    Active = old.Active,
                    MovingRight = old.MovingRight
                };
                smart.SetPlatforms(CurrentLevel.Platforms, CurrentLevel.FadingPlatforms);
                smart.InitializeAI(Pathfinder, old.PatrolStartX, old.PatrolEndX, CurrentLevel);
                smartEnemies.Add(smart);
                CurrentLevel.Enemies.Add(smart);
            }

            EnemiesChanged?.Invoke(CurrentLevel.Enemies);
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
            {
                if (enemy is SmartEnemy smart)
                {
                    bool playerAlive = !GameOver && !LevelCompleted;
                    smart.UpdateAI(deltaTime, Player.Position, playerAlive);
                    smart.Update(deltaTime);
                }
                else
                {
                    enemy.Update(deltaTime);
                }
            }

            foreach (var fp in CurrentLevel.FadingPlatforms)
                fp.Update(deltaTime);

            Time.Update(deltaTime);
            if (Time.CurrentTime <= 0)
            {
                GameOver = true;
                GameLost?.Invoke();
                return;
            }

            Player.Update(deltaTime, Player.Gravity);
            ApplyConveyorEffect(deltaTime);
            ResolvePlatformCollisionsOptimized();
            HandlePortals();
            HandleCoinCollection();
            HandleEnemyCollision();
            HandleSpikeCollision();
            HandleCheckpoint();
            CheckExit();

            var pos = Player.Position;
            var vel = Player.Velocity;
            Physics.ClampToWorldBounds(Player, 800, 480, ref pos, ref vel);
            Player.Position = pos;
            Player.Velocity = vel;

            if (Player.Position.Y < -50)
                Player.Position = new Vector2(Player.Position.X, -50);

            if (Player.Position.Y + Player.Height >= 479f && !playerFellToVoid)
            {
                playerFellToVoid = true;
                ApplyPenalty(10f, true);
                if (Player.Position.Y + Player.Height > 480)
                    Player.Position = new Vector2(Player.Position.X, 480 - Player.Height);
            }

            if (Player.Position.Y + Player.Height < 470f)
                playerFellToVoid = false;

            if (penaltyCooldown > 0) penaltyCooldown -= deltaTime;
            if (portalCooldown > 0) portalCooldown -= deltaTime;

            PlayerMoved?.Invoke(Player.Position);
        }

        private void ResolvePlatformCollisionsOptimized()
        {
            var playerPotentials = CollisionManager.GetPotentialCollisions(Player);
            Vector2 velocity = Player.Velocity;
            bool grounded = false;
            bool anyCollision = false;

            foreach (var platform in playerPotentials)
            {
                if (platform is Platform || (platform is FadingPlatform fp && fp.IsVisible))
                {
                    if (Physics.ResolveFloorCollision(Player, ref velocity, platform, out Vector2 newPos))
                    {
                        Player.Position = newPos;
                        velocity = new Vector2(velocity.X, 0);
                        grounded = true;
                        anyCollision = true;
                        continue;
                    }

                    var result = Physics.ResolvePlatformCollision(Player, velocity, platform);
                    if (result.HasValue)
                    {
                        Player.Position = result.Value.newPos;
                        velocity = result.Value.newVel;
                        if (result.Value.grounded)
                            grounded = true;
                        anyCollision = true;
                    }
                }
            }

            if (!anyCollision && Player.Position.Y + Player.Height >= 479f && velocity.Y >= 0)
            {
                Player.Position = new Vector2(Player.Position.X, 480 - Player.Height);
                velocity = new Vector2(velocity.X, 0);
                grounded = true;
            }

            Player.Velocity = velocity;
            Player.IsGrounded = grounded;
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

        private void HandlePortals()
        {
            if (portalCooldown > 0) return;

            for (int i = 0; i < CurrentLevel.Portals.Count; i++)
            {
                var portal = CurrentLevel.Portals[i];
                if (!portal.Active) continue;
                if (CollisionManager.CheckCollision(Player, portal))
                {
                    Vector2 dest = portal.TargetPosition;

                    if (portal.PartnerIndex >= 0 && portal.PartnerIndex < CurrentLevel.Portals.Count)
                    {
                        var partner = CurrentLevel.Portals[portal.PartnerIndex];
                        dest = new Vector2(
                            partner.Position.X + partner.Width / 2 - Player.Width / 2,
                            partner.Position.Y + partner.Height - Player.Height
                        );
                    }

                    Player.Position = dest;
                    Player.Velocity = new Vector2(Player.Velocity.X * 0.5f, 0);
                    portalCooldown = 1.5f;
                    PortalUsed?.Invoke();
                    break;
                }
            }
        }

        private void HandleCoinCollection()
        {
            foreach (var coin in CurrentLevel.Coins)
            {
                if (coin.Collected) continue;
                if (CollisionManager.CheckCollision(Player, coin))
                {
                    coin.Collected = true;
                    CollectedCoins++;

                    Debug.WriteLine($"[GameModel] Coin collected! Total: {CollectedCoins}/{TotalCoinsRequired}");

                    CoinsChanged?.Invoke(CollectedCoins);
                    CoinCollectedEvent?.Invoke(coin);

                    if (coin.Type == CoinType.Gold)
                        Time.AddSeconds(10);
                    else
                        Time.AddSeconds(5);
                }
            }
        }

        private void HandleEnemyCollision()
        {
            if (penaltyCooldown > 0) return;

            foreach (var enemy in CurrentLevel.Enemies)
            {
                if (!enemy.Active) continue;
                if (CollisionManager.CheckCollision(Player, enemy))
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
                if (CollisionManager.CheckCollision(Player, spike))
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
                if (CollisionManager.CheckCollision(Player, cp))
                {
                    cp.Activated = true;
                    CheckpointActivated?.Invoke();
                    CheckpointStateChanged?.Invoke(cp);
                }
            }
        }

        private void CheckExit()
        {
            if (CurrentLevel.Door == null) return;
            if (CollectedCoins >= CurrentLevel.RequiredCoins)
            {
                if (!CurrentLevel.Door.IsOpen)
                {
                    CurrentLevel.Door.IsOpen = true;
                    DoorStateChanged?.Invoke(true);
                    DoorOpened?.Invoke();
                }
                if (CollisionManager.CheckCollision(Player, CurrentLevel.Door))
                {
                    LevelCompleted = true;
                    LevelCompletedEvent?.Invoke();
                }
            }
        }

        private void ApplyPenalty(float seconds, bool respawn)
        {
            Time.SubtractSeconds(seconds);
            DamageTaken?.Invoke();

            if (respawn)
            {
                Vector2 respawnPoint = CurrentLevel.PlayerSpawn;
                foreach (var cp in CurrentLevel.Checkpoints)
                {
                    if (cp.Activated)
                        respawnPoint = new Vector2(cp.Position.X, cp.Position.Y - Player.Height - 5);
                }
                Player.Position = respawnPoint;
                Player.Velocity = Vector2.Zero;
                Player.IsGrounded = false;
                playerFellToVoid = false;
            }

            penaltyCooldown = 0.8f;
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
                Jumped?.Invoke();
            }
        }
    }
}