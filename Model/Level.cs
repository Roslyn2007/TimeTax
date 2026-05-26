using System.Collections.Generic;
using TimeTax.Model.Entities;

namespace TimeTax.Model
{
    public class Level
    {
        public List<Platform> Platforms { get; } = new List<Platform>();
        public List<Coin> Coins { get; } = new List<Coin>();
        public List<Enemy> Enemies { get; } = new List<Enemy>();
        public List<Spike> Spikes { get; } = new List<Spike>();
        public List<Checkpoint> Checkpoints { get; } = new List<Checkpoint>();
        public List<Portal> Portals { get; } = new List<Portal>();
        public List<Conveyor> Conveyors { get; } = new List<Conveyor>();
        public List<FadingPlatform> FadingPlatforms { get; } = new List<FadingPlatform>();
        public ExitDoor? Door { get; set; }

        public Vector2 PlayerSpawn { get; set; }
        public int RequiredCoins { get; set; } = 10;
        public float StartTime { get; set; } = 90f;
        public string Name { get; set; } = "Unknown";

        public void LoadLevel(int levelNumber)
        {
            Clear();

            switch (levelNumber)
            {
                case 1: BuildLevel1(); break;
                case 2: BuildLevel2(); break;
                case 3: BuildLevel3(); break;
                case 4: BuildLevel4(); break;
                case 5: BuildLevel5(); break;
                default: BuildLevel1(); break;
            }
        }

        private void Clear()
        {
            Platforms.Clear();
            Coins.Clear();
            Enemies.Clear();
            Spikes.Clear();
            Checkpoints.Clear();
            Portals.Clear();
            Conveyors.Clear();
            FadingPlatforms.Clear();
            Door = null;
        }

        private void BuildLevel1()
        {
            Name = "First Steps";
            StartTime = 45f;
            RequiredCoins = 5;

            Platforms.Add(new Platform { Position = new Vector2(0, 460), Width = 250, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(320, 440), Width = 150, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(550, 460), Width = 250, Height = 15 });

            Platforms.Add(new Platform { Position = new Vector2(150, 340), Width = 100, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(400, 300), Width = 100, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(650, 340), Width = 100, Height = 15 });

            Platforms.Add(new Platform { Position = new Vector2(280, 390), Width = 60, Height = 15 });

            PlayerSpawn = new Vector2(30, 435);

            Coins.Add(new Coin { Position = new Vector2(80, 440), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(180, 440), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(380, 420), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(480, 420), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(620, 440), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(720, 440), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(200, 320), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(450, 280), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(700, 320), Type = CoinType.Normal });

            Enemies.Add(new Enemy
            {
                Position = new Vector2(120, 436),
                SpawnPosition = new Vector2(120, 436),
                PatrolStartX = 20,
                PatrolEndX = 220,
                PatrolSpeed = 70f,
                Active = true,
                MovingRight = true
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(400, 416),
                SpawnPosition = new Vector2(400, 416),
                PatrolStartX = 330,
                PatrolEndX = 460,
                PatrolSpeed = 60f,
                Active = true,
                MovingRight = false
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(450, 276),
                SpawnPosition = new Vector2(450, 276),
                PatrolStartX = 410,
                PatrolEndX = 490,
                PatrolSpeed = 50f,
                Active = true,
                MovingRight = true
            });

            Checkpoints.Add(new Checkpoint { Position = new Vector2(400, 408) });

            Door = new ExitDoor { Position = new Vector2(620, 428), IsOpen = false };
        }

        private void BuildLevel2()
        {
            Name = "Spike Valley";
            StartTime = 40f;
            RequiredCoins = 6;

            Platforms.Add(new Platform { Position = new Vector2(0, 460), Width = 200, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(280, 440), Width = 120, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(480, 420), Width = 120, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(680, 460), Width = 120, Height = 15 });

            Platforms.Add(new Platform { Position = new Vector2(120, 360), Width = 90, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(350, 320), Width = 90, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(600, 360), Width = 90, Height = 15 });

            Platforms.Add(new Platform { Position = new Vector2(250, 220), Width = 80, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(500, 220), Width = 80, Height = 15 });

            Platforms.Add(new Platform { Position = new Vector2(200, 280), Width = 50, Height = 15 });

            PlayerSpawn = new Vector2(30, 435);

            Coins.Add(new Coin { Position = new Vector2(80, 440), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(160, 440), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(330, 420), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(420, 420), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(530, 400), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(720, 440), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(160, 340), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(390, 300), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(640, 340), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(290, 200), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(540, 200), Type = CoinType.Normal });

            Enemies.Add(new Enemy
            {
                Position = new Vector2(100, 436),
                SpawnPosition = new Vector2(100, 436),
                PatrolStartX = 20,
                PatrolEndX = 180,
                PatrolSpeed = 75f,
                Active = true,
                MovingRight = true
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(340, 416),
                SpawnPosition = new Vector2(340, 416),
                PatrolStartX = 290,
                PatrolEndX = 390,
                PatrolSpeed = 65f,
                Active = true,
                MovingRight = false
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(390, 296),
                SpawnPosition = new Vector2(390, 296),
                PatrolStartX = 360,
                PatrolEndX = 430,
                PatrolSpeed = 55f,
                Active = true,
                MovingRight = true
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(540, 196),
                SpawnPosition = new Vector2(540, 196),
                PatrolStartX = 510,
                PatrolEndX = 570,
                PatrolSpeed = 50f,
                Active = true,
                MovingRight = false
            });

            Spikes.Add(new Spike { Position = new Vector2(210, 450), Width = 24, Height = 10 });
            Spikes.Add(new Spike { Position = new Vector2(410, 430), Width = 24, Height = 10 });
            Spikes.Add(new Spike { Position = new Vector2(220, 350), Width = 20, Height = 10 });

            Conveyors.Add(new Conveyor
            {
                Position = new Vector2(490, 410),
                Width = 100,
                Height = 10,
                Direction = ConveyorDirection.Right,
                Speed = 80f
            });

            Checkpoints.Add(new Checkpoint { Position = new Vector2(390, 408) });
            Checkpoints.Add(new Checkpoint { Position = new Vector2(380, 288) });

            Door = new ExitDoor { Position = new Vector2(740, 428), IsOpen = false };
        }

        private void BuildLevel3()
        {
            Name = "Portal Maze";
            StartTime = 35f;
            RequiredCoins = 7;

            Platforms.Add(new Platform { Position = new Vector2(0, 460), Width = 180, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(250, 440), Width = 100, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(450, 420), Width = 100, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(650, 460), Width = 150, Height = 15 });

            Platforms.Add(new Platform { Position = new Vector2(100, 360), Width = 80, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(350, 340), Width = 80, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(600, 360), Width = 80, Height = 15 });

            Platforms.Add(new Platform { Position = new Vector2(200, 220), Width = 70, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(500, 220), Width = 70, Height = 15 });

            Platforms.Add(new Platform { Position = new Vector2(300, 280), Width = 50, Height = 15 });

            FadingPlatforms.Add(new FadingPlatform { Position = new Vector2(400, 260), Width = 70, Height = 15 });

            Portals.Add(new Portal
            {
                Position = new Vector2(120, 310),
                TargetPosition = new Vector2(535, 196),
                Active = true
            });
            Portals.Add(new Portal
            {
                Position = new Vector2(620, 290),
                TargetPosition = new Vector2(235, 196),
                Active = true
            });

            PlayerSpawn = new Vector2(30, 435);

            Coins.Add(new Coin { Position = new Vector2(60, 440), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(140, 440), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(290, 420), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(380, 420), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(490, 400), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(700, 440), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(130, 340), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(380, 320), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(640, 340), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(230, 200), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(540, 200), Type = CoinType.Normal });

            Enemies.Add(new Enemy
            {
                Position = new Vector2(90, 436),
                SpawnPosition = new Vector2(90, 436),
                PatrolStartX = 20,
                PatrolEndX = 160,
                PatrolSpeed = 85f,
                Active = true,
                MovingRight = true
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(300, 416),
                SpawnPosition = new Vector2(300, 416),
                PatrolStartX = 260,
                PatrolEndX = 340,
                PatrolSpeed = 75f,
                Active = true,
                MovingRight = false
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(390, 316),
                SpawnPosition = new Vector2(390, 316),
                PatrolStartX = 360,
                PatrolEndX = 420,
                PatrolSpeed = 65f,
                Active = true,
                MovingRight = true
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(535, 196),
                SpawnPosition = new Vector2(535, 196),
                PatrolStartX = 510,
                PatrolEndX = 560,
                PatrolSpeed = 55f,
                Active = true,
                MovingRight = false
            });

            Spikes.Add(new Spike { Position = new Vector2(190, 450), Width = 22, Height = 10 });
            Spikes.Add(new Spike { Position = new Vector2(360, 430), Width = 22, Height = 10 });
            Spikes.Add(new Spike { Position = new Vector2(200, 350), Width = 18, Height = 10 });

            Conveyors.Add(new Conveyor
            {
                Position = new Vector2(260, 430),
                Width = 80,
                Height = 10,
                Direction = ConveyorDirection.Left,
                Speed = 95f
            });

            Checkpoints.Add(new Checkpoint { Position = new Vector2(380, 408) });
            Checkpoints.Add(new Checkpoint { Position = new Vector2(390, 308) });

            Door = new ExitDoor { Position = new Vector2(740, 428), IsOpen = false };
        }

        private void BuildLevel4()
        {
            Name = "Conveyor Rush";
            StartTime = 30f;
            RequiredCoins = 8;

            Platforms.Add(new Platform { Position = new Vector2(0, 460), Width = 160, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(240, 440), Width = 100, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(440, 420), Width = 100, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(660, 460), Width = 140, Height = 15 });

            Platforms.Add(new Platform { Position = new Vector2(100, 360), Width = 70, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(350, 340), Width = 70, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(600, 360), Width = 70, Height = 15 });

            Platforms.Add(new Platform { Position = new Vector2(250, 220), Width = 60, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(500, 220), Width = 60, Height = 15 });

            Platforms.Add(new Platform { Position = new Vector2(200, 280), Width = 45, Height = 15 });

            FadingPlatforms.Add(new FadingPlatform { Position = new Vector2(350, 250), Width = 60, Height = 15 });
            FadingPlatforms.Add(new FadingPlatform { Position = new Vector2(150, 150), Width = 50, Height = 15 });

            Portals.Add(new Portal
            {
                Position = new Vector2(115, 300),
                TargetPosition = new Vector2(530, 196),
                Active = true
            });
            Portals.Add(new Portal
            {
                Position = new Vector2(620, 280),
                TargetPosition = new Vector2(275, 196),
                Active = true
            });

            PlayerSpawn = new Vector2(30, 435);

            Coins.Add(new Coin { Position = new Vector2(50, 440), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(130, 440), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(280, 420), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(380, 420), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(480, 400), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(700, 440), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(120, 340), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(380, 320), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(640, 340), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(280, 200), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(530, 200), Type = CoinType.Normal });

            Enemies.Add(new Enemy
            {
                Position = new Vector2(80, 436),
                SpawnPosition = new Vector2(80, 436),
                PatrolStartX = 20,
                PatrolEndX = 140,
                PatrolSpeed = 95f,
                Active = true,
                MovingRight = true
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(290, 416),
                SpawnPosition = new Vector2(290, 416),
                PatrolStartX = 250,
                PatrolEndX = 330,
                PatrolSpeed = 85f,
                Active = true,
                MovingRight = false
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(390, 316),
                SpawnPosition = new Vector2(390, 316),
                PatrolStartX = 360,
                PatrolEndX = 410,
                PatrolSpeed = 75f,
                Active = true,
                MovingRight = true
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(530, 196),
                SpawnPosition = new Vector2(530, 196),
                PatrolStartX = 510,
                PatrolEndX = 550,
                PatrolSpeed = 65f,
                Active = true,
                MovingRight = false
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(280, 196),
                SpawnPosition = new Vector2(280, 196),
                PatrolStartX = 260,
                PatrolEndX = 300,
                PatrolSpeed = 65f,
                Active = true,
                MovingRight = true
            });

            Spikes.Add(new Spike { Position = new Vector2(170, 450), Width = 20, Height = 10 });
            Spikes.Add(new Spike { Position = new Vector2(350, 430), Width = 20, Height = 10 });
            Spikes.Add(new Spike { Position = new Vector2(190, 350), Width = 16, Height = 10 });

            Conveyors.Add(new Conveyor
            {
                Position = new Vector2(250, 430),
                Width = 80,
                Height = 10,
                Direction = ConveyorDirection.Right,
                Speed = 105f
            });
            Conveyors.Add(new Conveyor
            {
                Position = new Vector2(440, 410),
                Width = 80,
                Height = 10,
                Direction = ConveyorDirection.Left,
                Speed = 105f
            });

            Checkpoints.Add(new Checkpoint { Position = new Vector2(370, 408) });
            Checkpoints.Add(new Checkpoint { Position = new Vector2(385, 308) });

            Door = new ExitDoor { Position = new Vector2(740, 428), IsOpen = false };
        }

        private void BuildLevel5()
        {
            Name = "Time Tax";
            StartTime = 25f;
            RequiredCoins = 9;

            Platforms.Add(new Platform { Position = new Vector2(0, 460), Width = 140, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(220, 440), Width = 90, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(420, 420), Width = 90, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(640, 460), Width = 160, Height = 15 });

            Platforms.Add(new Platform { Position = new Vector2(80, 360), Width = 65, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(320, 340), Width = 65, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(580, 360), Width = 65, Height = 15 });

            Platforms.Add(new Platform { Position = new Vector2(200, 220), Width = 55, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(500, 220), Width = 55, Height = 15 });

            Platforms.Add(new Platform { Position = new Vector2(350, 280), Width = 40, Height = 15 });

            FadingPlatforms.Add(new FadingPlatform { Position = new Vector2(300, 250), Width = 55, Height = 15 });
            FadingPlatforms.Add(new FadingPlatform { Position = new Vector2(180, 150), Width = 50, Height = 15 });
            FadingPlatforms.Add(new FadingPlatform { Position = new Vector2(480, 130), Width = 50, Height = 15 });

            Portals.Add(new Portal
            {
                Position = new Vector2(95, 290),
                TargetPosition = new Vector2(525, 196),
                Active = true
            });
            Portals.Add(new Portal
            {
                Position = new Vector2(610, 270),
                TargetPosition = new Vector2(225, 196),
                Active = true
            });

            PlayerSpawn = new Vector2(30, 435);

            Coins.Add(new Coin { Position = new Vector2(40, 440), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(110, 440), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(260, 420), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(360, 420), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(460, 400), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(680, 440), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(760, 440), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(100, 340), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(350, 320), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(610, 340), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(230, 200), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(530, 200), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(380, 110), Type = CoinType.Gold });

            Enemies.Add(new Enemy
            {
                Position = new Vector2(70, 436),
                SpawnPosition = new Vector2(70, 436),
                PatrolStartX = 10,
                PatrolEndX = 120,
                PatrolSpeed = 110f,
                Active = true,
                MovingRight = true
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(265, 416),
                SpawnPosition = new Vector2(265, 416),
                PatrolStartX = 230,
                PatrolEndX = 300,
                PatrolSpeed = 95f,
                Active = true,
                MovingRight = false
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(355, 316),
                SpawnPosition = new Vector2(355, 316),
                PatrolStartX = 330,
                PatrolEndX = 375,
                PatrolSpeed = 85f,
                Active = true,
                MovingRight = true
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(525, 196),
                SpawnPosition = new Vector2(525, 196),
                PatrolStartX = 505,
                PatrolEndX = 545,
                PatrolSpeed = 75f,
                Active = true,
                MovingRight = false
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(225, 196),
                SpawnPosition = new Vector2(225, 196),
                PatrolStartX = 205,
                PatrolEndX = 245,
                PatrolSpeed = 75f,
                Active = true,
                MovingRight = true
            });

            Spikes.Add(new Spike { Position = new Vector2(150, 450), Width = 16, Height = 10 });
            Spikes.Add(new Spike { Position = new Vector2(330, 430), Width = 16, Height = 10 });
            Spikes.Add(new Spike { Position = new Vector2(170, 350), Width = 14, Height = 10 });
            Spikes.Add(new Spike { Position = new Vector2(450, 350), Width = 14, Height = 10 });

            Conveyors.Add(new Conveyor
            {
                Position = new Vector2(230, 430),
                Width = 70,
                Height = 10,
                Direction = ConveyorDirection.Right,
                Speed = 125f
            });
            Conveyors.Add(new Conveyor
            {
                Position = new Vector2(420, 410),
                Width = 70,
                Height = 10,
                Direction = ConveyorDirection.Left,
                Speed = 125f
            });
            Conveyors.Add(new Conveyor
            {
                Position = new Vector2(320, 330),
                Width = 60,
                Height = 10,
                Direction = ConveyorDirection.Left,
                Speed = 135f
            });

            Checkpoints.Add(new Checkpoint { Position = new Vector2(365, 408) });
            Checkpoints.Add(new Checkpoint { Position = new Vector2(355, 308) });
            Checkpoints.Add(new Checkpoint { Position = new Vector2(430, 188) });

            Door = new ExitDoor { Position = new Vector2(740, 428), IsOpen = false };
        }
    }
}