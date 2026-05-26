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
        public string BackgroundFileName { get; set; } = "bg1";

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
            BackgroundFileName = "bg1";

            Platforms.Add(new Platform { Position = new Vector2(0, 460), Width = 300, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(350, 440), Width = 150, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(550, 460), Width = 250, Height = 20 });

            Platforms.Add(new Platform { Position = new Vector2(150, 360), Width = 120, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(400, 320), Width = 120, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(650, 360), Width = 120, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(280, 400), Width = 80, Height = 20 });

            PlayerSpawn = new Vector2(30, 440);

            Coins.Add(new Coin { Position = new Vector2(80, 430), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(200, 430), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(430, 410), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(480, 410), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(650, 430), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(750, 430), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(200, 330), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(450, 290), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(700, 330), Type = CoinType.Normal });

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
                PatrolEndX = 475,
                PatrolSpeed = 50f,
                Active = true,
                MovingRight = false
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(450, 296),
                SpawnPosition = new Vector2(450, 296),
                PatrolStartX = 410,
                PatrolEndX = 495,
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
            BackgroundFileName = "bg2";

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

            PlayerSpawn = new Vector2(30, 440);

            Coins.Add(new Coin { Position = new Vector2(80, 430), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(160, 430), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(300, 410), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(270, 410), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(520, 390), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(720, 430), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(160, 330), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(420, 290), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(640, 330), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(290, 210), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(540, 210), Type = CoinType.Normal });

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
                PatrolEndX = 365,
                PatrolSpeed = 55f,
                Active = true,
                MovingRight = false
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(390, 296),
                SpawnPosition = new Vector2(390, 296),
                PatrolStartX = 360,
                PatrolEndX = 425,
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

            Spikes.Add(new Spike { Position = new Vector2(50, 450), Width = 30, Height = 10 });

            Spikes.Add(new Spike { Position = new Vector2(280, 430), Width = 30, Height = 10 });

            Spikes.Add(new Spike { Position = new Vector2(140, 350), Width = 24, Height = 10 });

            Conveyors.Add(new Conveyor
            {
                Position = new Vector2(460, 410),
                Width = 120,
                Height = 10,
                Direction = ConveyorDirection.Right,
                Speed = 70f
            });

            Checkpoints.Add(new Checkpoint { Position = new Vector2(330, 408) });
            Checkpoints.Add(new Checkpoint { Position = new Vector2(380, 288) });

            Door = new ExitDoor { Position = new Vector2(720, 428), IsOpen = false };
        }

        private void BuildLevel3()
        {
            Name = "Portal Maze";
            StartTime = 40f;
            RequiredCoins = 7;
            BackgroundFileName = "bg3";

            Platforms.Add(new Platform { Position = new Vector2(0, 460), Width = 180, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(250, 440), Width = 120, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(450, 420), Width = 120, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(650, 460), Width = 150, Height = 20 });

            Platforms.Add(new Platform { Position = new Vector2(100, 360), Width = 90, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(350, 340), Width = 90, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(600, 360), Width = 90, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(200, 260), Width = 80, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(500, 260), Width = 80, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(250, 300), Width = 60, Height = 20 });

            FadingPlatforms.Add(new FadingPlatform { Position = new Vector2(400, 280), Width = 80, Height = 20 });

            Portals.Add(new Portal
            {
                Position = new Vector2(130, 320),
                TargetPosition = new Vector2(520, 240),
                PartnerIndex = 1,
                Active = true
            });
            Portals.Add(new Portal
            {
                Position = new Vector2(610, 320),
                TargetPosition = new Vector2(240, 240),
                PartnerIndex = 0,
                Active = true
            });

            PlayerSpawn = new Vector2(30, 440);

            Coins.Add(new Coin { Position = new Vector2(60, 430), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(140, 430), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(290, 410), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(270, 410), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(490, 390), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(700, 430), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(110, 330), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(350, 310), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(660, 330), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(210, 230), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(560, 230), Type = CoinType.Normal });

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
                PatrolEndX = 345,
                PatrolSpeed = 65f,
                Active = true,
                MovingRight = false
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(390, 316),
                SpawnPosition = new Vector2(390, 316),
                PatrolStartX = 360,
                PatrolEndX = 415,
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

            Spikes.Add(new Spike { Position = new Vector2(50, 450), Width = 26, Height = 10 });

            Spikes.Add(new Spike { Position = new Vector2(280, 430), Width = 26, Height = 10 });

            Spikes.Add(new Spike { Position = new Vector2(110, 350), Width = 22, Height = 10 });

            Conveyors.Add(new Conveyor
            {
                Position = new Vector2(260, 430),
                Width = 90,
                Height = 10,
                Direction = ConveyorDirection.Left,
                Speed = 85f
            });

            Checkpoints.Add(new Checkpoint { Position = new Vector2(330, 408) });
            Checkpoints.Add(new Checkpoint { Position = new Vector2(390, 308) });

            Door = new ExitDoor { Position = new Vector2(740, 428), IsOpen = false };
        }

        private void BuildLevel4()
        {
            Name = "Conveyor Rush";
            StartTime = 50f;
            RequiredCoins = 8;
            BackgroundFileName = "bg4";

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
                Position = new Vector2(130, 320),
                TargetPosition = new Vector2(620, 298),
                PartnerIndex = 1,
                Active = true
            });
            Portals.Add(new Portal
            {
                Position = new Vector2(620, 320),
                TargetPosition = new Vector2(130, 298),
                PartnerIndex = 0,
                Active = true
            });

            PlayerSpawn = new Vector2(30, 440);

            Coins.Add(new Coin { Position = new Vector2(50, 430), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(130, 430), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(280, 410), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(270, 410), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(480, 390), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(700, 430), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(170, 330), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(350, 310), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(660, 330), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(280, 230), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(530, 230), Type = CoinType.Normal });

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
                PatrolEndX = 325,
                PatrolSpeed = 75f,
                Active = true,
                MovingRight = false
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(390, 316),
                SpawnPosition = new Vector2(390, 316),
                PatrolStartX = 360,
                PatrolEndX = 405,
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

            Spikes.Add(new Spike { Position = new Vector2(40, 450), Width = 24, Height = 10 });

            Spikes.Add(new Spike { Position = new Vector2(100, 450), Width = 24, Height = 10 });

            Spikes.Add(new Spike { Position = new Vector2(110, 350), Width = 20, Height = 10 });

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

            Checkpoints.Add(new Checkpoint { Position = new Vector2(300, 408) });
            Checkpoints.Add(new Checkpoint { Position = new Vector2(385, 308) });

            Door = new ExitDoor { Position = new Vector2(740, 428), IsOpen = false };
        }

        private void BuildLevel5()
        {
            Name = "Time Tax";
            StartTime = 45f;
            RequiredCoins = 9;
            BackgroundFileName = "bg5";

            Platforms.Add(new Platform { Position = new Vector2(0, 460), Width = 140, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(220, 440), Width = 100, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(420, 420), Width = 100, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(640, 460), Width = 160, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(80, 360), Width = 70, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(320, 340), Width = 70, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(580, 360), Width = 70, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(200, 220), Width = 60, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(500, 220), Width = 60, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(350, 260), Width = 45, Height = 20 });

            FadingPlatforms.Add(new FadingPlatform { Position = new Vector2(260, 290), Width = 60, Height = 20 });
            FadingPlatforms.Add(new FadingPlatform { Position = new Vector2(180, 190), Width = 55, Height = 20 });
            FadingPlatforms.Add(new FadingPlatform { Position = new Vector2(480, 170), Width = 55, Height = 20 });

            Portals.Add(new Portal
            {
                Position = new Vector2(95, 320),
                TargetPosition = new Vector2(580, 298),
                PartnerIndex = 1,
                Active = true
            });
            Portals.Add(new Portal
            {
                Position = new Vector2(580, 320),
                TargetPosition = new Vector2(95, 298),
                PartnerIndex = 0,
                Active = true
            });

            PlayerSpawn = new Vector2(30, 440);

            Coins.Add(new Coin { Position = new Vector2(40, 430), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(110, 430), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(260, 410), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(280, 410), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(440, 390), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(680, 430), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(700, 430), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(130, 330), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(330, 310), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(630, 330), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(230, 205), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(510, 205), Type = CoinType.Normal });

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
                PatrolEndX = 295,
                PatrolSpeed = 85f,
                Active = true,
                MovingRight = false
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(355, 316),
                SpawnPosition = new Vector2(355, 316),
                PatrolStartX = 330,
                PatrolEndX = 365,
                PatrolSpeed = 75f,
                Active = true,
                MovingRight = true
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(525, 196),
                SpawnPosition = new Vector2(525, 196),
                PatrolStartX = 505,
                PatrolEndX = 550,
                PatrolSpeed = 65f,
                Active = true,
                MovingRight = false
            });
            Enemies.Add(new Enemy
            {
                Position = new Vector2(225, 196),
                SpawnPosition = new Vector2(225, 196),
                PatrolStartX = 205,
                PatrolEndX = 250,
                PatrolSpeed = 65f,
                Active = true,
                MovingRight = true
            });

            Spikes.Add(new Spike { Position = new Vector2(30, 450), Width = 20, Height = 10 });
            Spikes.Add(new Spike { Position = new Vector2(70, 450), Width = 20, Height = 10 });
            Spikes.Add(new Spike { Position = new Vector2(90, 350), Width = 18, Height = 10 });
            Spikes.Add(new Spike { Position = new Vector2(650, 450), Width = 18, Height = 10 });

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

            Checkpoints.Add(new Checkpoint { Position = new Vector2(470, 388) });
            Checkpoints.Add(new Checkpoint { Position = new Vector2(355, 308) });
            Checkpoints.Add(new Checkpoint { Position = new Vector2(530, 188) });

            Door = new ExitDoor { Position = new Vector2(740, 428), IsOpen = false };
        }
    }
}