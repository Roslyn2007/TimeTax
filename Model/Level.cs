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
            StartTime = 50f;
            RequiredCoins = 5;

            Platforms.Add(new Platform { Position = new Vector2(0, 460), Width = 300, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(350, 440), Width = 150, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(550, 460), Width = 250, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(150, 360), Width = 120, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(400, 320), Width = 120, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(650, 360), Width = 120, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(280, 400), Width = 80, Height = 20 });

            PlayerSpawn = new Vector2(30, 430);

            Coins.Add(new Coin { Position = new Vector2(80, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(200, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(400, 415), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(500, 415), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(650, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(750, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(200, 335), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(450, 295), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(700, 335), Type = CoinType.Normal });

            Enemies.Add(new Enemy
            {
                Position = new Vector2(120, 436),
                SpawnPosition = new Vector2(120, 436),
                PatrolStartX = 30,
                PatrolEndX = 270,
                PatrolSpeed = 60f,
                Active = true,
                MovingRight = true
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(400, 416),
                SpawnPosition = new Vector2(400, 416),
                PatrolStartX = 360,
                PatrolEndX = 490,
                PatrolSpeed = 50f,
                Active = true,
                MovingRight = false
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(450, 296),
                SpawnPosition = new Vector2(450, 296),
                PatrolStartX = 410,
                PatrolEndX = 510,
                PatrolSpeed = 45f,
                Active = true,
                MovingRight = true
            });

            Checkpoints.Add(new Checkpoint { Position = new Vector2(400, 408) });

            Door = new ExitDoor { Position = new Vector2(700, 428), IsOpen = false };
        }

        private void BuildLevel2()
        {
            Name = "Spike Valley";
            StartTime = 45f;
            RequiredCoins = 6;

            Platforms.Add(new Platform { Position = new Vector2(0, 460), Width = 200, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(250, 440), Width = 140, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(450, 420), Width = 140, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(680, 460), Width = 120, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(120, 360), Width = 100, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(350, 320), Width = 100, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(600, 360), Width = 100, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(250, 240), Width = 80, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(500, 240), Width = 80, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(200, 300), Width = 60, Height = 20 });

            PlayerSpawn = new Vector2(30, 430);

            Coins.Add(new Coin { Position = new Vector2(80, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(160, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(300, 415), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(400, 415), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(520, 395), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(720, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(160, 335), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(390, 295), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(640, 335), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(290, 215), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(540, 215), Type = CoinType.Normal });

            Enemies.Add(new Enemy
            {
                Position = new Vector2(100, 436),
                SpawnPosition = new Vector2(100, 436),
                PatrolStartX = 30,
                PatrolEndX = 170,
                PatrolSpeed = 65f,
                Active = true,
                MovingRight = true
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(340, 416),
                SpawnPosition = new Vector2(340, 416),
                PatrolStartX = 260,
                PatrolEndX = 380,
                PatrolSpeed = 55f,
                Active = true,
                MovingRight = false
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(390, 296),
                SpawnPosition = new Vector2(390, 296),
                PatrolStartX = 360,
                PatrolEndX = 440,
                PatrolSpeed = 50f,
                Active = true,
                MovingRight = true
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(540, 216),
                SpawnPosition = new Vector2(540, 216),
                PatrolStartX = 510,
                PatrolEndX = 570,
                PatrolSpeed = 45f,
                Active = true,
                MovingRight = false
            });

            Spikes.Add(new Spike { Position = new Vector2(210, 450), Width = 30, Height = 10 });
            Spikes.Add(new Spike { Position = new Vector2(410, 430), Width = 30, Height = 10 });
            Spikes.Add(new Spike { Position = new Vector2(220, 350), Width = 24, Height = 10 });

            Conveyors.Add(new Conveyor
            {
                Position = new Vector2(460, 410),
                Width = 120,
                Height = 10,
                Direction = ConveyorDirection.Right,
                Speed = 70f
            });

            Checkpoints.Add(new Checkpoint { Position = new Vector2(390, 408) });
            Checkpoints.Add(new Checkpoint { Position = new Vector2(380, 288) });

            Door = new ExitDoor { Position = new Vector2(720, 428), IsOpen = false };
        }

        private void BuildLevel3()
        {
            Name = "Portal Maze";
            StartTime = 40f;
            RequiredCoins = 7;

            Platforms.Add(new Platform { Position = new Vector2(0, 460), Width = 180, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(250, 440), Width = 120, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(450, 420), Width = 120, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(650, 460), Width = 150, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(100, 360), Width = 90, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(350, 340), Width = 90, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(600, 360), Width = 90, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(200, 260), Width = 80, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(500, 260), Width = 80, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(300, 300), Width = 60, Height = 20 });

            FadingPlatforms.Add(new FadingPlatform { Position = new Vector2(400, 280), Width = 80, Height = 20 });

            Portals.Add(new Portal
            {
                Position = new Vector2(120, 310),
                TargetPosition = new Vector2(540, 236),
                Active = true
            });
            Portals.Add(new Portal
            {
                Position = new Vector2(620, 290),
                TargetPosition = new Vector2(240, 236),
                Active = true
            });

            PlayerSpawn = new Vector2(30, 430);

            Coins.Add(new Coin { Position = new Vector2(60, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(140, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(290, 415), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(380, 415), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(490, 395), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(700, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(130, 335), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(380, 315), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(640, 335), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(230, 235), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(540, 235), Type = CoinType.Normal });

            Enemies.Add(new Enemy
            {
                Position = new Vector2(90, 436),
                SpawnPosition = new Vector2(90, 436),
                PatrolStartX = 20,
                PatrolEndX = 160,
                PatrolSpeed = 75f,
                Active = true,
                MovingRight = true
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(300, 416),
                SpawnPosition = new Vector2(300, 416),
                PatrolStartX = 260,
                PatrolEndX = 360,
                PatrolSpeed = 65f,
                Active = true,
                MovingRight = false
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(390, 316),
                SpawnPosition = new Vector2(390, 316),
                PatrolStartX = 360,
                PatrolEndX = 430,
                PatrolSpeed = 55f,
                Active = true,
                MovingRight = true
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(540, 236),
                SpawnPosition = new Vector2(540, 236),
                PatrolStartX = 510,
                PatrolEndX = 570,
                PatrolSpeed = 50f,
                Active = true,
                MovingRight = false
            });

            Spikes.Add(new Spike { Position = new Vector2(190, 450), Width = 26, Height = 10 });
            Spikes.Add(new Spike { Position = new Vector2(360, 430), Width = 26, Height = 10 });
            Spikes.Add(new Spike { Position = new Vector2(200, 350), Width = 22, Height = 10 });

            Conveyors.Add(new Conveyor
            {
                Position = new Vector2(260, 430),
                Width = 90,
                Height = 10,
                Direction = ConveyorDirection.Left,
                Speed = 85f
            });

            Checkpoints.Add(new Checkpoint { Position = new Vector2(380, 408) });
            Checkpoints.Add(new Checkpoint { Position = new Vector2(390, 308) });

            Door = new ExitDoor { Position = new Vector2(740, 428), IsOpen = false };
        }

        private void BuildLevel4()
        {
            Name = "Conveyor Rush";
            StartTime = 35f;
            RequiredCoins = 8;

            Platforms.Add(new Platform { Position = new Vector2(0, 460), Width = 160, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(240, 440), Width = 110, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(440, 420), Width = 110, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(660, 460), Width = 140, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(100, 360), Width = 80, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(350, 340), Width = 80, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(600, 360), Width = 80, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(250, 260), Width = 70, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(500, 260), Width = 70, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(200, 300), Width = 50, Height = 20 });

            FadingPlatforms.Add(new FadingPlatform { Position = new Vector2(350, 290), Width = 70, Height = 20 });
            FadingPlatforms.Add(new FadingPlatform { Position = new Vector2(150, 190), Width = 60, Height = 20 });

            Portals.Add(new Portal
            {
                Position = new Vector2(115, 300),
                TargetPosition = new Vector2(535, 236),
                Active = true
            });
            Portals.Add(new Portal
            {
                Position = new Vector2(620, 280),
                TargetPosition = new Vector2(285, 236),
                Active = true
            });

            PlayerSpawn = new Vector2(30, 430);

            Coins.Add(new Coin { Position = new Vector2(50, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(130, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(280, 415), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(380, 415), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(480, 395), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(700, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(120, 335), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(380, 315), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(640, 335), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(280, 235), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(530, 235), Type = CoinType.Normal });

            Enemies.Add(new Enemy
            {
                Position = new Vector2(80, 436),
                SpawnPosition = new Vector2(80, 436),
                PatrolStartX = 20,
                PatrolEndX = 140,
                PatrolSpeed = 85f,
                Active = true,
                MovingRight = true
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(290, 416),
                SpawnPosition = new Vector2(290, 416),
                PatrolStartX = 250,
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
                Position = new Vector2(530, 236),
                SpawnPosition = new Vector2(530, 236),
                PatrolStartX = 510,
                PatrolEndX = 560,
                PatrolSpeed = 55f,
                Active = true,
                MovingRight = false
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(280, 236),
                SpawnPosition = new Vector2(280, 236),
                PatrolStartX = 260,
                PatrolEndX = 310,
                PatrolSpeed = 55f,
                Active = true,
                MovingRight = true
            });

            Spikes.Add(new Spike { Position = new Vector2(170, 450), Width = 24, Height = 10 });
            Spikes.Add(new Spike { Position = new Vector2(350, 430), Width = 24, Height = 10 });
            Spikes.Add(new Spike { Position = new Vector2(190, 350), Width = 20, Height = 10 });

            Conveyors.Add(new Conveyor
            {
                Position = new Vector2(250, 430),
                Width = 90,
                Height = 10,
                Direction = ConveyorDirection.Right,
                Speed = 95f
            });
            Conveyors.Add(new Conveyor
            {
                Position = new Vector2(440, 410),
                Width = 90,
                Height = 10,
                Direction = ConveyorDirection.Left,
                Speed = 95f
            });

            Checkpoints.Add(new Checkpoint { Position = new Vector2(370, 408) });
            Checkpoints.Add(new Checkpoint { Position = new Vector2(385, 308) });

            Door = new ExitDoor { Position = new Vector2(740, 428), IsOpen = false };
        }

        private void BuildLevel5()
        {
            Name = "Time Tax";
            StartTime = 30f;
            RequiredCoins = 9;

            Platforms.Add(new Platform { Position = new Vector2(0, 460), Width = 140, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(220, 440), Width = 100, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(420, 420), Width = 100, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(640, 460), Width = 160, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(80, 360), Width = 70, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(320, 340), Width = 70, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(580, 360), Width = 70, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(200, 260), Width = 60, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(500, 260), Width = 60, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(350, 300), Width = 45, Height = 20 });

            FadingPlatforms.Add(new FadingPlatform { Position = new Vector2(300, 290), Width = 60, Height = 20 });
            FadingPlatforms.Add(new FadingPlatform { Position = new Vector2(180, 190), Width = 55, Height = 20 });
            FadingPlatforms.Add(new FadingPlatform { Position = new Vector2(480, 170), Width = 55, Height = 20 });

            Portals.Add(new Portal
            {
                Position = new Vector2(95, 290),
                TargetPosition = new Vector2(525, 236),
                Active = true
            });
            Portals.Add(new Portal
            {
                Position = new Vector2(610, 270),
                TargetPosition = new Vector2(225, 236),
                Active = true
            });

            PlayerSpawn = new Vector2(30, 430);

            Coins.Add(new Coin { Position = new Vector2(40, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(110, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(260, 415), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(360, 415), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(460, 395), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(680, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(760, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(100, 335), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(350, 315), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(610, 335), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(230, 235), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(530, 235), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(380, 145), Type = CoinType.Gold });

            Enemies.Add(new Enemy
            {
                Position = new Vector2(70, 436),
                SpawnPosition = new Vector2(70, 436),
                PatrolStartX = 10,
                PatrolEndX = 120,
                PatrolSpeed = 95f,
                Active = true,
                MovingRight = true
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(265, 416),
                SpawnPosition = new Vector2(265, 416),
                PatrolStartX = 230,
                PatrolEndX = 310,
                PatrolSpeed = 85f,
                Active = true,
                MovingRight = false
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(355, 316),
                SpawnPosition = new Vector2(355, 316),
                PatrolStartX = 330,
                PatrolEndX = 385,
                PatrolSpeed = 75f,
                Active = true,
                MovingRight = true
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(525, 236),
                SpawnPosition = new Vector2(525, 236),
                PatrolStartX = 505,
                PatrolEndX = 550,
                PatrolSpeed = 65f,
                Active = true,
                MovingRight = false
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(225, 236),
                SpawnPosition = new Vector2(225, 236),
                PatrolStartX = 205,
                PatrolEndX = 250,
                PatrolSpeed = 65f,
                Active = true,
                MovingRight = true
            });

            Spikes.Add(new Spike { Position = new Vector2(150, 450), Width = 20, Height = 10 });
            Spikes.Add(new Spike { Position = new Vector2(330, 430), Width = 20, Height = 10 });
            Spikes.Add(new Spike { Position = new Vector2(170, 350), Width = 18, Height = 10 });
            Spikes.Add(new Spike { Position = new Vector2(450, 350), Width = 18, Height = 10 });

            Conveyors.Add(new Conveyor
            {
                Position = new Vector2(230, 430),
                Width = 80,
                Height = 10,
                Direction = ConveyorDirection.Right,
                Speed = 110f
            });
            Conveyors.Add(new Conveyor
            {
                Position = new Vector2(420, 410),
                Width = 80,
                Height = 10,
                Direction = ConveyorDirection.Left,
                Speed = 110f
            });
            Conveyors.Add(new Conveyor
            {
                Position = new Vector2(320, 330),
                Width = 70,
                Height = 10,
                Direction = ConveyorDirection.Left,
                Speed = 120f
            });

            Checkpoints.Add(new Checkpoint { Position = new Vector2(365, 408) });
            Checkpoints.Add(new Checkpoint { Position = new Vector2(355, 308) });
            Checkpoints.Add(new Checkpoint { Position = new Vector2(430, 228) });

            Door = new ExitDoor { Position = new Vector2(740, 428), IsOpen = false };
        }
    }
}