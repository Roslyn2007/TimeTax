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
        public ExitDoor Door { get; set; }

        public Vector2 PlayerSpawn { get; set; }
        public int RequiredCoins { get; set; } = 10;
        public float StartTime { get; set; } = 90f;

        // Простая платформа
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

        public void LoadTestLevel()
        {
            // Пол
            Platforms.Add(new Platform { Position = new Vector2(0, 460), Width = 800, Height = 20 });

            // Несколько платформ
            Platforms.Add(new Platform { Position = new Vector2(200, 400), Width = 100, Height = 10 });
            Platforms.Add(new Platform { Position = new Vector2(500, 350), Width = 100, Height = 10 });

            // Монеты
            Coins.Add(new Coin { Position = new Vector2(220, 375), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(250, 375), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(280, 375), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(520, 325), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(550, 325), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(580, 325), Type = CoinType.Normal });
            Coins.Add(new Coin { Position = new Vector2(100, 435), Type = CoinType.Gold });

            // Враг
            Enemies.Add(new Enemy { Position = new Vector2(400, 440) });

            // Шипы
            Spikes.Add(new Spike { Position = new Vector2(300, 445), Width = 40, Height = 10 });

            // Чекпоинт
            Checkpoints.Add(new Checkpoint { Position = new Vector2(600, 430) });

            // Выход (закроется, пока не собрано нужное число монет)
            Door = new ExitDoor { Position = new Vector2(750, 430), IsOpen = false };

            // Спавн игрока
            PlayerSpawn = new Vector2(50, 400);

            RequiredCoins = 7;
            StartTime = 90f;
        }
    }
}