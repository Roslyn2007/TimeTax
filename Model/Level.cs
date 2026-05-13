using System.Collections.Generic;
using TimeTax.Model.Entities;
using TimeTax.Model.Interfaces;

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
        public ExitDoor Door { get; set; }

        public Vector2 PlayerSpawn { get; set; }
        public int RequiredCoins { get; set; } = 10;
        public float StartTime { get; set; } = 90f;
        public string Name { get; set; } = "Unknown";

        public class Platform : ICollidable
        {
            public Vector2 Position { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }

            public (float left, float right, float top, float bottom) GetBounds()
            {
                float left = Position.X;
                float right = Position.X + Width;
                float top = Position.Y;
                float bottom = Position.Y + Height;
                return (left, right, top, bottom);
            }
        }

        public void LoadLevel(int levelNumber)
        {
            Clear();
            switch (levelNumber)
            {
                case 1: LoadLevel1(); break;
                case 2: LoadLevel2(); break;
                case 3: LoadLevel3(); break;
                case 4: LoadLevel4(); break;
                case 5: LoadLevel5(); break;
                default: LoadLevel1(); break;
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
            Door = null;
        }

        private void LoadLevel1()
        {
            Name = "Tax Office";
            StartTime = 90f;
            RequiredCoins = 7;
            PlayerSpawn = new Vector2(50, 400);

            Platforms.Add(new Platform { Position = new Vector2(0, 460), Width = 800, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(200, 380), Width = 120, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(500, 340), Width = 120, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(350, 280), Width = 100, Height = 15 });

            Coins.Add(new Coin { Position = new Vector2(230, 355), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(260, 355), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(290, 355), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(530, 315), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(560, 315), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(590, 315), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(100, 435), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(380, 255), Type = CoinType.Normal });

            Enemies.Add(new Enemy { Position = new Vector2(400, 440), PatrolStartX = 350, PatrolEndX = 500, PatrolSpeed = 50f });

            Spikes.Add(new Spike { Position = new Vector2(300, 445), Width = 40, Height = 10 });

            Checkpoints.Add(new Checkpoint { Position = new Vector2(600, 430) });

            Door = new ExitDoor { Position = new Vector2(750, 430), IsOpen = false };
        }

        private void LoadLevel2()
        {
            Name = "Clockwork";
            StartTime = 80f;
            RequiredCoins = 8;
            PlayerSpawn = new Vector2(50, 400);

            Platforms.Add(new Platform { Position = new Vector2(0, 460), Width = 200, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(300, 460), Width = 500, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(200, 360), Width = 100, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(400, 300), Width = 100, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(600, 240), Width = 100, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(100, 180), Width = 120, Height = 15 });

            Coins.Add(new Coin { Position = new Vector2(240, 335), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(440, 275), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(640, 215), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(140, 155), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(350, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(450, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(550, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(650, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(50, 435), Type = CoinType.Normal });

            Enemies.Add(new Enemy { Position = new Vector2(350, 440), PatrolStartX = 320, PatrolEndX = 480, PatrolSpeed = 70f });
            Enemies.Add(new Enemy { Position = new Vector2(550, 440), PatrolStartX = 520, PatrolEndX = 680, PatrolSpeed = 70f });

            Spikes.Add(new Spike { Position = new Vector2(280, 445), Width = 20, Height = 10 });
            Spikes.Add(new Spike { Position = new Vector2(500, 445), Width = 20, Height = 10 });

            Checkpoints.Add(new Checkpoint { Position = new Vector2(420, 270) });

            Door = new ExitDoor { Position = new Vector2(720, 430), IsOpen = false };
        }

        private void LoadLevel3()
        {
            Name = "Time Maze";
            StartTime = 75f;
            RequiredCoins = 10;
            PlayerSpawn = new Vector2(50, 400);

            Platforms.Add(new Platform { Position = new Vector2(0, 460), Width = 250, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(350, 460), Width = 450, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(50, 340), Width = 100, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(150, 270), Width = 100, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(80, 190), Width = 100, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(200, 140), Width = 120, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(450, 370), Width = 80, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(550, 310), Width = 80, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(650, 250), Width = 120, Height = 15 });

            Coins.Add(new Coin { Position = new Vector2(80, 315), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(100, 315), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(180, 245), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(200, 245), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(110, 165), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(130, 165), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(240, 115), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(270, 115), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(470, 345), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(570, 285), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(690, 225), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(720, 225), Type = CoinType.Normal });

            Enemies.Add(new Enemy { Position = new Vector2(400, 440), PatrolStartX = 380, PatrolEndX = 500, PatrolSpeed = 90f });
            Enemies.Add(new Enemy { Position = new Vector2(670, 240), PatrolStartX = 660, PatrolEndX = 720, PatrolSpeed = 60f });

            Spikes.Add(new Spike { Position = new Vector2(300, 445), Width = 30, Height = 10 });

            Checkpoints.Add(new Checkpoint { Position = new Vector2(230, 120) });
            Checkpoints.Add(new Checkpoint { Position = new Vector2(680, 220) });

            Door = new ExitDoor { Position = new Vector2(750, 430), IsOpen = false };
        }

        // УРОВЕНЬ 4 — ИСПРАВЛЕННЫЙ
        private void LoadLevel4()
        {
            Name = "Time Factory";
            StartTime = 70f;
            RequiredCoins = 12;
            PlayerSpawn = new Vector2(50, 400);

            // Левый пол (старт)
            Platforms.Add(new Platform { Position = new Vector2(0, 460), Width = 180, Height = 20 });
            // Правый пол
            Platforms.Add(new Platform { Position = new Vector2(300, 460), Width = 500, Height = 20 });
            
            // Платформа над конвейером 1 (левая)
            Platforms.Add(new Platform { Position = new Vector2(180, 380), Width = 100, Height = 15 });
            // Платформа посередине
            Platforms.Add(new Platform { Position = new Vector2(350, 320), Width = 100, Height = 15 });
            // Платформа над конвейером 2 (правая)
            Platforms.Add(new Platform { Position = new Vector2(550, 360), Width = 100, Height = 15 });
            // Верхняя платформа (куда ведёт портал)
            Platforms.Add(new Platform { Position = new Vector2(600, 220), Width = 120, Height = 15 });
            // Левая верхняя
            Platforms.Add(new Platform { Position = new Vector2(50, 200), Width = 100, Height = 15 });

            // Конвейер 1: под платформой 180,380 — толкает ВПРАВО к платформе 350,320
            Conveyors.Add(new Conveyor { 
                Position = new Vector2(180, 460), 
                Width = 120, 
                Height = 10, 
                Direction = ConveyorDirection.Right, 
                Speed = 100f 
            });
            
            // Конвейер 2: под платформой 550,360 — толкает ВЛЕВО (ловушка!)
            Conveyors.Add(new Conveyor { 
                Position = new Vector2(500, 460), 
                Width = 100, 
                Height = 10, 
                Direction = ConveyorDirection.Left, 
                Speed = 80f 
            });

            // Портал: с платформы 350,320 → на верхнюю 600,220
            // Размещаем портал на уровне пола, куда можно упасть с платформы
            Portals.Add(new Portal { 
                Position = new Vector2(370, 280),  // Между платформами
                TargetPosition = new Vector2(630, 180)  // На верхнюю платформу
            });

            // Монеты
            Coins.Add(new Coin { Position = new Vector2(210, 355), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(230, 355), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(380, 295), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(400, 295), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(580, 335), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(600, 335), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(640, 195), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(660, 195), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(80, 175), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(100, 175), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(320, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(420, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(520, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(620, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(720, 435), Type = CoinType.Gold });

            // Враги
            Enemies.Add(new Enemy { Position = new Vector2(350, 440), PatrolStartX = 300, PatrolEndX = 450, PatrolSpeed = 80f });
            Enemies.Add(new Enemy { Position = new Vector2(550, 440), PatrolStartX = 500, PatrolEndX = 650, PatrolSpeed = 80f });

            // Шипы — в ямах между платформами
            Spikes.Add(new Spike { Position = new Vector2(180, 445), Width = 30, Height = 10 });
            Spikes.Add(new Spike { Position = new Vector2(480, 445), Width = 30, Height = 10 });

            Checkpoints.Add(new Checkpoint { Position = new Vector2(630, 190) });

            Door = new ExitDoor { Position = new Vector2(750, 430), IsOpen = false };
        }

        private void LoadLevel5()
        {
            Name = "Time Vault";
            StartTime = 65f;
            RequiredCoins = 15;
            PlayerSpawn = new Vector2(50, 400);

            Platforms.Add(new Platform { Position = new Vector2(0, 460), Width = 800, Height = 20 });
            Platforms.Add(new Platform { Position = new Vector2(150, 360), Width = 120, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(350, 300), Width = 120, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(550, 240), Width = 120, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(200, 180), Width = 120, Height = 15 });
            Platforms.Add(new Platform { Position = new Vector2(500, 130), Width = 180, Height = 15 });

            Coins.Add(new Coin { Position = new Vector2(180, 335), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(200, 335), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(380, 275), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(400, 275), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(580, 215), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(600, 215), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(230, 155), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(250, 155), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(530, 105), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(550, 105), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(570, 105), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(100, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(150, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(300, 435), Type = CoinType.Gold });
            Coins.Add(new Coin { Position = new Vector2(400, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(500, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(600, 435), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(700, 435), Type = CoinType.Gold });

            Enemies.Add(new Enemy { Position = new Vector2(300, 440), PatrolStartX = 200, PatrolEndX = 400, PatrolSpeed = 120f });
            Enemies.Add(new Enemy { Position = new Vector2(500, 440), PatrolStartX = 450, PatrolEndX = 600, PatrolSpeed = 100f });
            Enemies.Add(new Enemy { Position = new Vector2(400, 280), PatrolStartX = 350, PatrolEndX = 450, PatrolSpeed = 90f });

            Spikes.Add(new Spike { Position = new Vector2(250, 445), Width = 40, Height = 10 });
            Spikes.Add(new Spike { Position = new Vector2(450, 445), Width = 40, Height = 10 });
            Spikes.Add(new Spike { Position = new Vector2(650, 445), Width = 40, Height = 10 });

            Checkpoints.Add(new Checkpoint { Position = new Vector2(560, 210) });
            Checkpoints.Add(new Checkpoint { Position = new Vector2(530, 100) });

            Door = new ExitDoor { Position = new Vector2(720, 100), IsOpen = false };
        }
    }
}