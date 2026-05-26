using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using TimeTax.Model.Entities;

namespace TimeTax.Model.Generation
{
    public class ProceduralLevelGenerator
    {
        private Random random = null!;
        private int seed;
        private int levelNumber;

        private const int WorldWidth = 800;
        private const int WorldHeight = 480;
        private const int GroundY = 460;

        private const float PlayerWidth = 20f;
        private const float PlayerHeight = 20f;
        private const float MaxJumpHeight = 100f;
        private const float MaxJumpDistance = 120f;
        private const float MaxSafeFall = 180f;

        private const float MinPlatformWidth = 100f;
        private const float MaxPlatformWidth = 180f;
        private const float PlatformHeight = 15f;
        private const float MinGap = 25f;
        private const float MaxGap = 90f;

        private const int CellWidth = 100;
        private const int CellHeight = 70;
        private const int GridCols = 8;
        private const int GridRows = 5;

        public Level GenerateLevel(int lvlNumber, int? customSeed = null)
        {
            levelNumber = lvlNumber;

            for (int attempt = 0; attempt < 50; attempt++)
            {
                seed = (customSeed ?? Environment.TickCount) + attempt * 7919;
                random = new Random(seed);

                var level = TryBuildLevel(lvlNumber);
                if (level != null)
                {
                    Debug.WriteLine($"[Gen] Level {lvlNumber} generated, attempt {attempt}, seed {seed}");
                    return level;
                }
            }

            Debug.WriteLine($"[Gen] Level {lvlNumber} FALLBACK used");
            return CreateFallbackLevel(lvlNumber);
        }

        private Level? TryBuildLevel(int lvlNumber)
        {
            try
            {
                var path = GenerateSolutionPath();
                if (path == null || path.Count < 3)
                    return null;

                var level = new Level();
                var mainPlatforms = BuildMainPlatforms(path);

                if (mainPlatforms == null || mainPlatforms.Count < 3)
                    return null;

                level.Platforms.AddRange(mainPlatforms);
                AddDecorativePlatforms(level, mainPlatforms);

                PlacePlayerAndDoor(level, mainPlatforms);
                PlacePortals(level, mainPlatforms, lvlNumber);
                PlaceConveyors(level, mainPlatforms, lvlNumber);
                PlaceFadingPlatforms(level, mainPlatforms, lvlNumber);

                PlaceCoins(level, mainPlatforms, lvlNumber);
                PlaceEnemies(level, mainPlatforms, lvlNumber);
                PlaceSpikes(level, mainPlatforms, lvlNumber);
                PlaceCheckpoints(level, mainPlatforms, lvlNumber);

                level.Name = $"Level {lvlNumber}";
                level.StartTime = Math.Max(50, 95 - lvlNumber * 8);

                int targetCoins = 4 + lvlNumber * 2;
                level.RequiredCoins = Math.Min(targetCoins, Math.Max(3, level.Coins.Count - 1));

                if (!QuickValidate(level, mainPlatforms))
                {
                    Debug.WriteLine($"[Gen] Validation failed for seed {seed}");
                    return null;
                }

                return level;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Gen] Exception: {ex.Message}");
                return null;
            }
        }

        private List<(int col, int row)>? GenerateSolutionPath()
        {
            var path = new List<(int col, int row)>();

            int startCol = 0;
            int startRow = GridRows - 1;
            int endCol = GridCols - 1;
            int endRow = random.Next(0, Math.Min(3, GridRows));

            int currentCol = startCol;
            int currentRow = startRow;
            path.Add((currentCol, currentRow));

            var visited = new HashSet<(int, int)>();
            visited.Add((currentCol, currentRow));

            while (currentCol < endCol)
            {
                int nextCol = currentCol + 1;
                int nextRow = currentRow;

                if (random.Next(100) < 30 && currentRow > 0)
                    nextRow = currentRow - 1;
                else if (random.Next(100) < 20 && currentRow < GridRows - 1)
                    nextRow = currentRow + 1;

                int rowDiff = Math.Abs(nextRow - currentRow);
                if (rowDiff > 1)
                    nextRow = currentRow;

                if (visited.Contains((nextCol, nextRow)))
                    nextRow = currentRow;

                if (visited.Contains((nextCol, nextRow)))
                {
                    currentCol++;
                    if (currentCol >= GridCols) break;
                    continue;
                }

                currentCol = nextCol;
                currentRow = nextRow;
                path.Add((currentCol, currentRow));
                visited.Add((currentCol, currentRow));

                if (currentCol >= endCol)
                    break;
            }

            if (path.Last().col < endCol)
                path.Add((endCol, endRow));

            if (path.Count >= 2 && path[path.Count - 2].col == endCol && path[path.Count - 2].row == endRow)
                path.RemoveAt(path.Count - 1);

            return path.Count >= 3 ? path : null;
        }

        private List<Platform>? BuildMainPlatforms(List<(int col, int row)> path)
        {
            var platforms = new List<Platform>();

            for (int i = 0; i < path.Count; i++)
            {
                var cell = path[i];

                float baseX = cell.col * CellWidth;
                float baseY = 100 + cell.row * CellHeight;

                float x = baseX + random.Next(-10, 11);
                float y = baseY + random.Next(-8, 9);
                float width = random.Next((int)MinPlatformWidth, (int)MaxPlatformWidth);

                if (i == 0 || i == path.Count - 1)
                    width = 140f;

                if (x < 0) x = 0;
                if (x + width > WorldWidth - 10)
                    width = WorldWidth - 10 - x;
                if (width < MinPlatformWidth)
                    width = MinPlatformWidth;

                if (i > 0)
                {
                    var prev = platforms[i - 1];
                    float prevRight = prev.Position.X + prev.Width;
                    float gap = x - prevRight;

                    if (gap < MinGap)
                    {
                        x = prevRight + MinGap + random.Next(5, 15);
                        if (x + width > WorldWidth - 10)
                            width = WorldWidth - 10 - x;
                    }
                    else if (gap > MaxGap)
                    {
                        x = prevRight + MaxGap - random.Next(10, 20);
                        if (x < prevRight + MinGap)
                            x = prevRight + MinGap + 5;
                    }
                    else if (gap < 0)
                    {
                        x = prevRight + MinGap + random.Next(5, 15);
                    }

                    float heightDiff = prev.Position.Y - y;
                    if (heightDiff > MaxJumpHeight * 0.5f)
                        y = prev.Position.Y - MaxJumpHeight * 0.45f;
                    else if (heightDiff < -MaxSafeFall * 0.4f)
                        y = prev.Position.Y + MaxSafeFall * 0.35f;

                    float finalGap = x - (prev.Position.X + prev.Width);
                    float finalHeightDiff = Math.Abs(prev.Position.Y - y);
                    float diagonal = MathF.Sqrt(finalGap * finalGap + finalHeightDiff * finalHeightDiff);
                    float maxReach = MaxJumpDistance * 0.85f;
                    if (diagonal > maxReach && finalGap > 40f)
                    {
                        x = prevRight + Math.Min(finalGap * 0.7f, MaxGap - 10);
                        if (x + width > WorldWidth - 10)
                            width = WorldWidth - 10 - x;
                    }
                }

                if (y < 120f) y = 120f;
                if (y > GroundY - 20) y = GroundY - 20;

                if (i == path.Count - 1)
                {
                    x = Math.Min(x, WorldWidth - 130);
                    if (x + width < WorldWidth - 80)
                        width = Math.Max(width, WorldWidth - 80 - x);
                }

                platforms.Add(new Platform
                {
                    Position = new Vector2(x, y),
                    Width = width,
                    Height = PlatformHeight
                });
            }

            var last = platforms.Last();
            float lastRight = last.Position.X + last.Width;
            if (lastRight < WorldWidth - 70)
            {
                float finalX = WorldWidth - 90;
                float finalY = last.Position.Y;
                float gap = finalX - lastRight;

                if (gap > MaxGap)
                {
                    float midX = lastRight + (MaxGap - 20);
                    platforms.Add(new Platform
                    {
                        Position = new Vector2(midX, finalY),
                        Width = 90,
                        Height = PlatformHeight
                    });
                }

                platforms.Add(new Platform
                {
                    Position = new Vector2(finalX, finalY),
                    Width = 80,
                    Height = PlatformHeight
                });
            }

            return platforms;
        }

        private void AddDecorativePlatforms(Level level, List<Platform> mainPath)
        {
            if (mainPath.Count <= 2) return;

            int count = 1 + levelNumber;
            int added = 0;

            for (int attempts = 0; attempts < 40 && added < count; attempts++)
            {
                var basePlat = mainPath[random.Next(1, mainPath.Count - 1)];

                float heightOffset = random.Next(30, 56);
                float y = basePlat.Position.Y - heightOffset;

                float maxHOffset = Math.Min(50f, MaxJumpDistance * 0.4f);
                float x = basePlat.Position.X + random.Next((int)-maxHOffset, (int)(basePlat.Width + maxHOffset + 1));
                float width = random.Next(60, 100);

                if (x < 20) x = 20;
                if (x + width > WorldWidth - 60) x = WorldWidth - 60 - width;
                if (y < 100) y = 100;
                if (y > GroundY - 30) y = GroundY - 30;

                float baseCenterX = basePlat.Position.X + basePlat.Width / 2;
                float thisCenterX = x + width / 2;
                float hDist = Math.Abs(thisCenterX - baseCenterX);
                float vDist = basePlat.Position.Y - y;
                float diag = MathF.Sqrt(hDist * hDist + vDist * vDist);
                if (diag > MaxJumpDistance * 0.65f)
                    continue;

                bool intersects = false;
                foreach (var plat in level.Platforms)
                {
                    float pRight = plat.Position.X + plat.Width;
                    float nRight = x + width;

                    bool overlapX = plat.Position.X < nRight && pRight > x;
                    bool closeY = Math.Abs(plat.Position.Y - y) < PlayerHeight + 8;

                    if (overlapX && closeY)
                    {
                        intersects = true;
                        break;
                    }
                }

                if (intersects) continue;

                level.Platforms.Add(new Platform
                {
                    Position = new Vector2(x, y),
                    Width = width,
                    Height = PlatformHeight
                });
                added++;
            }
        }

        private void PlacePlayerAndDoor(Level level, List<Platform> mainPath)
        {
            var start = mainPath.First();
            var end = mainPath.Last();

            const float doorWidth = 24f;
            const float sideClearance = 22f;
            float minPlatWidth = doorWidth + sideClearance * 2;

            if (end.Width < minPlatWidth)
            {
                end.Width = minPlatWidth;
                if (end.Position.X + end.Width > WorldWidth - 10)
                    end.Position = new Vector2(WorldWidth - 10 - end.Width, end.Position.Y);
            }

            float doorX = end.Position.X + end.Width / 2 - doorWidth / 2;
            float doorY = end.Position.Y - 32;

            var doorWalkRect = new Rect((int)(doorX - sideClearance), (int)(doorY - 5),
                                         (int)(doorWidth + sideClearance * 2), 42);

            foreach (var plat in level.Platforms)
            {
                if (plat == end) continue;
                var pb = plat.GetBounds();
                var platRect = new Rect((int)pb.left - 2, (int)pb.top - 2,
                    (int)(pb.right - pb.left) + 4, (int)(pb.bottom - pb.top) + 4);

                if (doorWalkRect.Intersects(platRect))
                {
                    doorX = end.Position.X + sideClearance;
                    doorWalkRect = new Rect((int)(doorX - sideClearance), (int)(doorY - 5),
                                             (int)(doorWidth + sideClearance * 2), 42);
                    break;
                }
            }

            foreach (var plat in level.Platforms)
            {
                if (plat == end) continue;
                var pb = plat.GetBounds();
                var platRect = new Rect((int)pb.left - 2, (int)pb.top - 2,
                    (int)(pb.right - pb.left) + 4, (int)(pb.bottom - pb.top) + 4);
                if (doorWalkRect.Intersects(platRect))
                {
                    doorX = end.Position.X + end.Width - doorWidth - sideClearance;
                    break;
                }
            }

            doorX = Clamp(doorX, 5, WorldWidth - 35);
            doorY = Clamp(doorY, 50, GroundY - 40);

            level.PlayerSpawn = new Vector2(
                start.Position.X + 35,
                start.Position.Y - PlayerHeight - 5
            );

            level.Door = new ExitDoor
            {
                Position = new Vector2(doorX, doorY),
                IsOpen = false
            };
        }

        private void PlaceCoins(Level level, List<Platform> mainPath, int lvlNumber)
        {
            int targetCoins = 6 + lvlNumber * 3;
            int goldChance = Math.Min(25 + lvlNumber * 10, 50);

            var allPlats = new List<Platform>(mainPath);
            allPlats.AddRange(level.Platforms.Except(mainPath));

            var occupiedRects = new List<Rect>();

            foreach (var portal in level.Portals)
            {
                occupiedRects.Add(new Rect(
                    (int)portal.Position.X - 10, (int)portal.Position.Y - 10,
                    (int)portal.Width + 20, (int)portal.Height + 20));
            }

            foreach (var spike in level.Spikes)
            {
                var sb = spike.GetBounds();
                occupiedRects.Add(new Rect((int)sb.left - 2, (int)sb.top - 2,
                    (int)(sb.right - sb.left) + 4, (int)(sb.bottom - sb.top) + 4));
            }

            foreach (var enemy in level.Enemies)
            {
                var eb = enemy.GetBounds();
                occupiedRects.Add(new Rect((int)eb.left - 5, (int)eb.top - 5,
                    (int)(eb.right - eb.left) + 10, (int)(eb.bottom - eb.top) + 10));
            }

            foreach (var cp in level.Checkpoints)
            {
                var cb = cp.GetBounds();
                occupiedRects.Add(new Rect((int)cb.left - 5, (int)cb.top - 5,
                    (int)(cb.right - cb.left) + 10, (int)(cb.bottom - cb.top) + 10));
            }

            foreach (var conv in level.Conveyors)
            {
                var cnb = conv.GetBounds();
                occupiedRects.Add(new Rect((int)cnb.left - 2, (int)cnb.top - 2,
                    (int)(cnb.right - cnb.left) + 4, (int)(cnb.bottom - cnb.top) + 4));
            }

            if (level.Door != null)
            {
                var db = level.Door.GetBounds();
                occupiedRects.Add(new Rect((int)db.left - 5, (int)db.top - 5,
                    (int)(db.right - db.left) + 10, (int)(db.bottom - db.top) + 10));
            }

            for (int i = 1; i < allPlats.Count - 1 && targetCoins > 0; i++)
            {
                var plat = allPlats[i];
                int count = Math.Min(2, targetCoins);
                if (plat.Position.Y < 200) count = Math.Min(3, targetCoins);

                for (int c = 0; c < count; c++)
                {
                    float x = plat.Position.X + 20 + c * 28;
                    if (x > plat.Position.X + plat.Width - 15) break;
                    float y = plat.Position.Y - 20;

                    if (x < 5 || x > WorldWidth - 5 || y < 5 || y > GroundY - 10) continue;

                    var coinRect = new Rect((int)x - 15, (int)y - 15, 30, 30);
                    if (occupiedRects.Any(r => r.Intersects(coinRect))) continue;

                    level.Coins.Add(new Coin
                    {
                        Position = new Vector2(x, y),
                        Type = random.Next(100) < goldChance ? CoinType.Gold : CoinType.Normal
                    });
                    targetCoins--;
                    occupiedRects.Add(coinRect);
                }
            }

            int airAttempts = 0;
            while (targetCoins > 0 && airAttempts < 100)
            {
                airAttempts++;
                int idx = random.Next(0, mainPath.Count - 1);
                var plat = mainPath[idx];
                var nextPlat = mainPath[idx + 1];

                float midX = (plat.Position.X + plat.Width + nextPlat.Position.X) / 2;
                float midY = Math.Min(plat.Position.Y, nextPlat.Position.Y) - random.Next(30, 70);

                if (midX < 10 || midX > WorldWidth - 10 || midY < 10 || midY > GroundY - 20) continue;

                var coinRect = new Rect((int)midX - 15, (int)midY - 15, 30, 30);
                if (occupiedRects.Any(r => r.Intersects(coinRect))) continue;

                level.Coins.Add(new Coin
                {
                    Position = new Vector2(midX, midY),
                    Type = random.Next(100) < goldChance ? CoinType.Gold : CoinType.Normal
                });
                targetCoins--;
                occupiedRects.Add(coinRect);
            }
        }

        private void PlaceEnemies(Level level, List<Platform> mainPath, int lvlNumber)
        {
            var allPlats = new List<Platform>(mainPath);
            allPlats.AddRange(level.Platforms.Except(mainPath));

            int maxPossible = Math.Max(0, allPlats.Count - 1);
            int desired = Math.Max(2, 1 + lvlNumber);
            int count = Math.Min(desired, maxPossible);

            if (count <= 0) return;

            var occupiedRects = new List<Rect>();

            foreach (var portal in level.Portals)
            {
                occupiedRects.Add(new Rect(
                    (int)portal.Position.X - 15, (int)portal.Position.Y - 15,
                    (int)portal.Width + 30, (int)portal.Height + 30));
            }

            foreach (var spike in level.Spikes)
            {
                var sb = spike.GetBounds();
                occupiedRects.Add(new Rect((int)sb.left - 5, (int)sb.top - 5,
                    (int)(sb.right - sb.left) + 10, (int)(sb.bottom - sb.top) + 10));
            }

            foreach (var coin in level.Coins)
            {
                occupiedRects.Add(new Rect((int)coin.Position.X - 10, (int)coin.Position.Y - 10, 35, 35));
            }

            foreach (var cp in level.Checkpoints)
            {
                var cb = cp.GetBounds();
                occupiedRects.Add(new Rect((int)cb.left - 5, (int)cb.top - 5,
                    (int)(cb.right - cb.left) + 10, (int)(cb.bottom - cb.top) + 10));
            }

            foreach (var conv in level.Conveyors)
            {
                var cnb = conv.GetBounds();
                occupiedRects.Add(new Rect((int)cnb.left - 5, (int)cnb.top - 5,
                    (int)(cnb.right - cnb.left) + 10, (int)(cnb.bottom - cnb.top) + 10));
            }

            if (level.Door != null)
            {
                var db = level.Door.GetBounds();
                occupiedRects.Add(new Rect((int)db.left - 10, (int)db.top - 10,
                    (int)(db.right - db.left) + 20, (int)(db.bottom - db.top) + 20));
            }

            int placed = 0;
            int totalAttempts = 0;
            var usedPlatforms = new HashSet<int>();

            while (placed < count && totalAttempts < 80)
            {
                totalAttempts++;

                int idx;
                if (allPlats.Count - 1 >= count)
                {
                    int pickAttempts = 0;
                    do
                    {
                        idx = random.Next(1, allPlats.Count);
                        pickAttempts++;
                    }
                    while (usedPlatforms.Contains(idx) && pickAttempts < 15);

                    if (pickAttempts >= 15 && usedPlatforms.Count >= allPlats.Count - 1)
                        idx = random.Next(1, allPlats.Count);
                }
                else
                {
                    idx = random.Next(1, allPlats.Count);
                }

                usedPlatforms.Add(idx);
                var plat = allPlats[idx];

                float margin = 18;
                float startX = plat.Position.X + margin;
                float endX = plat.Position.X + plat.Width - margin;
                if (endX - startX < 40) continue;

                float x = startX + random.Next((int)(endX - startX));
                float y = plat.Position.Y - 24;

                if (x < 0 || x > WorldWidth - 24) continue;
                if (y < 0 || y > GroundY - 24) continue;

                var enemyRect = new Rect((int)x - 5, (int)y - 5, 34, 34);
                if (occupiedRects.Any(r => r.Intersects(enemyRect))) continue;

                var enemy = new Enemy
                {
                    Position = new Vector2(x, y),
                    SpawnPosition = new Vector2(x, y),  // <-- SPAWN POSITION SET
                    PatrolStartX = startX,
                    PatrolEndX = endX,
                    PatrolSpeed = 35f + lvlNumber * 18f + random.Next(20),
                    Active = true,
                    MovingRight = random.Next(2) == 0
                };
                level.Enemies.Add(enemy);
                occupiedRects.Add(enemyRect);
                placed++;
            }
        }

        private void PlaceSpikes(Level level, List<Platform> mainPath, int lvlNumber)
        {
            var allPlats = new List<Platform>(mainPath);
            allPlats.AddRange(level.Platforms.Except(mainPath));

            int count = Math.Min(lvlNumber, allPlats.Count / 2);

            var occupiedRects = new List<Rect>();

            foreach (var portal in level.Portals)
            {
                occupiedRects.Add(new Rect(
                    (int)portal.Position.X - 15, (int)portal.Position.Y - 15,
                    (int)portal.Width + 30, (int)portal.Height + 30));
            }

            foreach (var enemy in level.Enemies)
            {
                var eb = enemy.GetBounds();
                occupiedRects.Add(new Rect((int)eb.left - 5, (int)eb.top - 5,
                    (int)(eb.right - eb.left) + 10, (int)(eb.bottom - eb.top) + 10));
            }

            foreach (var coin in level.Coins)
            {
                occupiedRects.Add(new Rect((int)coin.Position.X - 10, (int)coin.Position.Y - 10, 35, 35));
            }

            foreach (var cp in level.Checkpoints)
            {
                var cb = cp.GetBounds();
                occupiedRects.Add(new Rect((int)cb.left - 5, (int)cb.top - 5,
                    (int)(cb.right - cb.left) + 10, (int)(cb.bottom - cb.top) + 10));
            }

            foreach (var conv in level.Conveyors)
            {
                var cnb = conv.GetBounds();
                occupiedRects.Add(new Rect((int)cnb.left - 5, (int)cnb.top - 5,
                    (int)(cnb.right - cnb.left) + 10, (int)(cnb.bottom - cnb.top) + 10));
            }

            if (level.Door != null)
            {
                var db = level.Door.GetBounds();
                occupiedRects.Add(new Rect((int)db.left - 10, (int)db.top - 10,
                    (int)(db.right - db.left) + 20, (int)(db.bottom - db.top) + 20));
            }

            for (int i = 0; i < count; i++)
            {
                var plat = allPlats[random.Next(1, allPlats.Count - 1)];
                float w = random.Next(20, Math.Min(40, (int)plat.Width - 40));
                float x = plat.Position.X + random.Next(15, (int)Math.Max(16, plat.Width - w - 15));
                float y = plat.Position.Y - 10;

                if (x < 0 || x + w > WorldWidth) continue;
                if (y < 0 || y > GroundY) continue;

                var spikeRect = new Rect((int)x - 5, (int)y - 5, (int)w + 10, 20);
                if (occupiedRects.Any(r => r.Intersects(spikeRect))) continue;

                level.Spikes.Add(new Spike
                {
                    Position = new Vector2(x, y),
                    Width = w,
                    Height = 10
                });
                occupiedRects.Add(spikeRect);
            }
        }

        private void PlaceCheckpoints(Level level, List<Platform> mainPath, int lvlNumber)
        {
            int cpCount = Math.Min(1 + lvlNumber / 2, 3);

            var occupiedRects = new List<Rect>();

            foreach (var portal in level.Portals)
            {
                occupiedRects.Add(new Rect(
                    (int)portal.Position.X - 15, (int)portal.Position.Y - 15,
                    (int)portal.Width + 30, (int)portal.Height + 30));
            }

            foreach (var coin in level.Coins)
            {
                occupiedRects.Add(new Rect((int)coin.Position.X - 10, (int)coin.Position.Y - 10, 35, 35));
            }

            foreach (var enemy in level.Enemies)
            {
                var eb = enemy.GetBounds();
                occupiedRects.Add(new Rect((int)eb.left - 5, (int)eb.top - 5,
                    (int)(eb.right - eb.left) + 10, (int)(eb.bottom - eb.top) + 10));
            }

            foreach (var spike in level.Spikes)
            {
                var sb = spike.GetBounds();
                occupiedRects.Add(new Rect((int)sb.left - 5, (int)sb.top - 5,
                    (int)(sb.right - sb.left) + 10, (int)(sb.bottom - sb.top) + 10));
            }

            foreach (var conv in level.Conveyors)
            {
                var cnb = conv.GetBounds();
                occupiedRects.Add(new Rect((int)cnb.left - 5, (int)cnb.top - 5,
                    (int)(cnb.right - cnb.left) + 10, (int)(cnb.bottom - cnb.top) + 10));
            }

            if (level.Door != null)
            {
                var db = level.Door.GetBounds();
                occupiedRects.Add(new Rect((int)db.left - 10, (int)db.top - 10,
                    (int)(db.right - db.left) + 20, (int)(db.bottom - db.top) + 20));
            }

            for (int i = 0; i < cpCount; i++)
            {
                int idx = (mainPath.Count - 2) * (i + 1) / (cpCount + 1);
                idx = Math.Max(1, Math.Min(idx, mainPath.Count - 3));
                var plat = mainPath[idx];

                float x = plat.Position.X + plat.Width / 2 - 12;
                float y = plat.Position.Y - 32;

                if (x < 0) x = 0;
                if (x > WorldWidth - 24) x = WorldWidth - 24;
                if (y < 0) y = 0;

                var cpRect = new Rect((int)x - 5, (int)y - 5, 34, 42);
                if (occupiedRects.Any(r => r.Intersects(cpRect))) continue;

                level.Checkpoints.Add(new Checkpoint
                {
                    Position = new Vector2(x, y)
                });
                occupiedRects.Add(cpRect);
            }
        }

        private void PlacePortals(Level level, List<Platform> mainPath, int lvlNumber)
        {
            if (lvlNumber < 2 || mainPath.Count < 4) return;

            int pairs = Math.Min(lvlNumber - 1, 2);

            var occupiedRects = new List<Rect>();

            foreach (var coin in level.Coins)
            {
                occupiedRects.Add(new Rect((int)coin.Position.X - 15, (int)coin.Position.Y - 15, 45, 45));
            }

            foreach (var spike in level.Spikes)
            {
                var sb = spike.GetBounds();
                occupiedRects.Add(new Rect((int)sb.left - 10, (int)sb.top - 10,
                    (int)(sb.right - sb.left) + 20, (int)(sb.bottom - sb.top) + 20));
            }

            foreach (var cp in level.Checkpoints)
            {
                var cb = cp.GetBounds();
                occupiedRects.Add(new Rect((int)cb.left - 10, (int)cb.top - 10,
                    (int)(cb.right - cb.left) + 20, (int)(cb.bottom - cb.top) + 20));
            }

            if (level.Door != null)
            {
                var db = level.Door.GetBounds();
                occupiedRects.Add(new Rect((int)db.left - 15, (int)db.top - 15,
                    (int)(db.right - db.left) + 30, (int)(db.bottom - db.top) + 30));
            }

            for (int i = 0; i < pairs; i++)
            {
                int p1 = random.Next(1, mainPath.Count / 2);
                int p2 = random.Next(mainPath.Count / 2, mainPath.Count - 1);

                var plat1 = mainPath[p1];
                var plat2 = mainPath[p2];

                float x1 = plat1.Position.X + plat1.Width / 2 - 15;
                float y1 = plat1.Position.Y - 40;
                float x2 = plat2.Position.X + plat2.Width / 2 - 15;
                float y2 = plat2.Position.Y - 40;

                x1 = Clamp(x1, 5, WorldWidth - 35);
                y1 = Clamp(y1, 5, GroundY - 45);
                x2 = Clamp(x2, 5, WorldWidth - 35);
                y2 = Clamp(y2, 5, GroundY - 45);

                var portal1Rect = new Rect((int)x1 - 5, (int)y1 - 5, 40, 50);
                var portal2Rect = new Rect((int)x2 - 5, (int)y2 - 5, 40, 50);

                if (occupiedRects.Any(r => r.Intersects(portal1Rect)) || occupiedRects.Any(r => r.Intersects(portal2Rect)))
                    continue;

                level.Portals.Add(new Portal
                {
                    Position = new Vector2(x1, y1),
                    TargetPosition = new Vector2(plat2.Position.X + plat2.Width / 2, plat2.Position.Y - 20),
                    Active = true
                });
                level.Portals.Add(new Portal
                {
                    Position = new Vector2(x2, y2),
                    TargetPosition = new Vector2(plat1.Position.X + plat1.Width / 2, plat1.Position.Y - 20),
                    Active = true
                });

                occupiedRects.Add(portal1Rect);
                occupiedRects.Add(portal2Rect);
            }
        }

        private void PlaceConveyors(Level level, List<Platform> mainPath, int lvlNumber)
        {
            if (lvlNumber < 2) return;

            var allPlats = new List<Platform>(mainPath);
            allPlats.AddRange(level.Platforms.Except(mainPath));

            int count = Math.Min(lvlNumber - 1, 3);

            var occupiedRects = new List<Rect>();

            foreach (var portal in level.Portals)
            {
                occupiedRects.Add(new Rect(
                    (int)portal.Position.X - 15, (int)portal.Position.Y - 15,
                    (int)portal.Width + 30, (int)portal.Height + 30));
            }

            foreach (var coin in level.Coins)
            {
                occupiedRects.Add(new Rect((int)coin.Position.X - 10, (int)coin.Position.Y - 10, 35, 35));
            }

            foreach (var enemy in level.Enemies)
            {
                var eb = enemy.GetBounds();
                occupiedRects.Add(new Rect((int)eb.left - 5, (int)eb.top - 5,
                    (int)(eb.right - eb.left) + 10, (int)(eb.bottom - eb.top) + 10));
            }

            foreach (var spike in level.Spikes)
            {
                var sb = spike.GetBounds();
                occupiedRects.Add(new Rect((int)sb.left - 5, (int)sb.top - 5,
                    (int)(sb.right - sb.left) + 10, (int)(sb.bottom - sb.top) + 10));
            }

            foreach (var cp in level.Checkpoints)
            {
                var cb = cp.GetBounds();
                occupiedRects.Add(new Rect((int)cb.left - 5, (int)cb.top - 5,
                    (int)(cb.right - cb.left) + 10, (int)(cb.bottom - cb.top) + 10));
            }

            if (level.Door != null)
            {
                var db = level.Door.GetBounds();
                occupiedRects.Add(new Rect((int)db.left - 10, (int)db.top - 10,
                    (int)(db.right - db.left) + 20, (int)(db.bottom - db.top) + 20));
            }

            for (int i = 0; i < count; i++)
            {
                var plat = allPlats[random.Next(1, allPlats.Count - 1)];
                float w = Math.Min(80, plat.Width - 30);
                float x = plat.Position.X + (plat.Width - w) / 2;
                float y = plat.Position.Y - 10;

                if (x < 0) x = 0;
                if (x + w > WorldWidth) w = WorldWidth - x;
                if (y < 0 || y > GroundY) continue;

                var convRect = new Rect((int)x - 5, (int)y - 5, (int)w + 10, 20);
                if (occupiedRects.Any(r => r.Intersects(convRect))) continue;

                level.Conveyors.Add(new Conveyor
                {
                    Position = new Vector2(x, y),
                    Width = w,
                    Height = 10,
                    Direction = random.Next(2) == 0 ? ConveyorDirection.Left : ConveyorDirection.Right,
                    Speed = 40f + lvlNumber * 20f
                });
                occupiedRects.Add(convRect);
            }
        }

        private void PlaceFadingPlatforms(Level level, List<Platform> mainPath, int lvlNumber)
        {
            if (lvlNumber < 3) return;

            int count = Math.Min(lvlNumber - 2, 4);

            var occupiedRects = new List<Rect>();

            foreach (var portal in level.Portals)
            {
                occupiedRects.Add(new Rect(
                    (int)portal.Position.X - 15, (int)portal.Position.Y - 15,
                    (int)portal.Width + 30, (int)portal.Height + 30));
            }

            foreach (var coin in level.Coins)
            {
                occupiedRects.Add(new Rect((int)coin.Position.X - 10, (int)coin.Position.Y - 10, 35, 35));
            }

            foreach (var enemy in level.Enemies)
            {
                var eb = enemy.GetBounds();
                occupiedRects.Add(new Rect((int)eb.left - 5, (int)eb.top - 5,
                    (int)(eb.right - eb.left) + 10, (int)(eb.bottom - eb.top) + 10));
            }

            foreach (var spike in level.Spikes)
            {
                var sb = spike.GetBounds();
                occupiedRects.Add(new Rect((int)sb.left - 5, (int)sb.top - 5,
                    (int)(sb.right - sb.left) + 10, (int)(sb.bottom - sb.top) + 10));
            }

            foreach (var cp in level.Checkpoints)
            {
                var cb = cp.GetBounds();
                occupiedRects.Add(new Rect((int)cb.left - 5, (int)cb.top - 5,
                    (int)(cb.right - cb.left) + 10, (int)(cb.bottom - cb.top) + 10));
            }

            foreach (var conv in level.Conveyors)
            {
                var cnb = conv.GetBounds();
                occupiedRects.Add(new Rect((int)cnb.left - 5, (int)cnb.top - 5,
                    (int)(cnb.right - cnb.left) + 10, (int)(cnb.bottom - cnb.top) + 10));
            }

            if (level.Door != null)
            {
                var db = level.Door.GetBounds();
                occupiedRects.Add(new Rect((int)db.left - 10, (int)db.top - 10,
                    (int)(db.right - db.left) + 20, (int)(db.bottom - db.top) + 20));
            }

            for (int i = 0; i < count; i++)
            {
                int idx = random.Next(2, mainPath.Count - 2);
                var basePlat = mainPath[idx];

                float x = basePlat.Position.X + basePlat.Width + random.Next(20, 50);
                float y = basePlat.Position.Y + random.Next(-25, 26);

                if (x < 0) x = 0;
                if (x > WorldWidth - 100) x = WorldWidth - 100;
                if (y < 80) y = 80;
                if (y > GroundY - 20) y = GroundY - 20;

                float w = random.Next(50, 90);

                var fpRect = new Rect((int)x - 5, (int)y - 5, (int)w + 10, 25);
                if (occupiedRects.Any(r => r.Intersects(fpRect))) continue;

                level.FadingPlatforms.Add(new FadingPlatform
                {
                    Position = new Vector2(x, y),
                    Width = w,
                    Height = PlatformHeight
                });
                occupiedRects.Add(fpRect);
            }
        }

        private bool QuickValidate(Level level, List<Platform> mainPath)
        {
            if (level.Door == null) return false;

            var door = level.Door.GetBounds();
            if (door.left < 0 || door.right > WorldWidth || door.top < 0 || door.bottom > WorldHeight)
                return false;

            bool doorOnPlatform = false;
            Platform? doorPlatform = null;
            foreach (var plat in level.Platforms)
            {
                var pb = plat.GetBounds();
                if (door.left >= pb.left - 5 && door.right <= pb.right + 5 &&
                    Math.Abs(door.bottom - pb.top) < 12)
                {
                    doorOnPlatform = true;
                    doorPlatform = plat;
                }

                var doorRect = new Rect((int)door.left - 2, (int)door.top - 2,
                    (int)(door.right - door.left) + 4, (int)(door.bottom - door.top) + 4);
                var platRect = new Rect((int)pb.left, (int)pb.top,
                    (int)(pb.right - pb.left), (int)(pb.bottom - pb.top));
                if (plat != doorPlatform && doorRect.Intersects(platRect))
                    return false;
            }
            if (!doorOnPlatform) return false;

            bool blockedLeft = false, blockedRight = false;
            foreach (var plat in level.Platforms)
            {
                if (plat == doorPlatform) continue;
                var pb = plat.GetBounds();
                if (pb.right > door.left - PlayerWidth - 2 && pb.left < door.left &&
                    pb.top < door.bottom && pb.bottom > door.top)
                    blockedLeft = true;
                if (pb.left < door.right + PlayerWidth + 2 && pb.right > door.right &&
                    pb.top < door.bottom && pb.bottom > door.top)
                    blockedRight = true;
            }
            if (blockedLeft && blockedRight) return false;

            var start = mainPath.First();
            if (level.PlayerSpawn.X < start.Position.X ||
                level.PlayerSpawn.X > start.Position.X + start.Width ||
                level.PlayerSpawn.Y < start.Position.Y - 40 ||
                level.PlayerSpawn.Y > start.Position.Y + 5)
                return false;

            for (int i = 0; i < mainPath.Count - 1; i++)
            {
                var a = mainPath[i];
                var b = mainPath[i + 1];

                float aRight = a.Position.X + a.Width;
                float bLeft = b.Position.X;
                float gap = bLeft - aRight;

                if (gap > MaxJumpDistance + 10) return false;

                float heightDiff = a.Position.Y - b.Position.Y;
                if (heightDiff > MaxJumpHeight * 0.6f && gap > 50) return false;
            }

            foreach (var dec in level.Platforms.Except(mainPath))
            {
                bool reachable = false;
                foreach (var main in mainPath)
                {
                    float hDist = Math.Abs((dec.Position.X + dec.Width / 2) - (main.Position.X + main.Width / 2));
                    float vDist = Math.Abs(dec.Position.Y - main.Position.Y);
                    float diag = MathF.Sqrt(hDist * hDist + vDist * vDist);
                    if (diag <= MaxJumpDistance * 0.75f)
                    {
                        reachable = true;
                        break;
                    }
                }
                if (!reachable) return false;
            }

            foreach (var coin in level.Coins)
            {
                if (coin.Position.X < 0 || coin.Position.X > WorldWidth ||
                    coin.Position.Y < 0 || coin.Position.Y > GroundY)
                    return false;
            }

            foreach (var portal in level.Portals)
            {
                if (portal.Position.X < 0 || portal.Position.X + portal.Width > WorldWidth ||
                    portal.Position.Y < 0 || portal.Position.Y + portal.Height > GroundY)
                    return false;
            }

            foreach (var enemy in level.Enemies)
            {
                if (enemy.Position.X < 0 || enemy.Position.X > WorldWidth ||
                    enemy.Position.Y < 0 || enemy.Position.Y > GroundY)
                    return false;
            }

            if (level.Coins.Count < level.RequiredCoins + 1) return false;
            if (level.Enemies.Count < 2) return false;

            return true;
        }

        private Level CreateFallbackLevel(int lvlNumber)
        {
            var level = new Level();
            float x = 0, y = GroundY;

            for (int i = 0; i < 8; i++)
            {
                level.Platforms.Add(new Platform
                {
                    Position = new Vector2(x, y),
                    Width = 100,
                    Height = PlatformHeight
                });
                x += 85;
                y -= 30;
                if (y < 160) y = 160;
            }

            level.Platforms.Add(new Platform
            {
                Position = new Vector2(WorldWidth - 80, y),
                Width = 80,
                Height = PlatformHeight
            });

            level.PlayerSpawn = new Vector2(30, GroundY - 25);
            level.Door = new ExitDoor
            {
                Position = new Vector2(WorldWidth - 80 + 28, y - 32),
                IsOpen = false
            };

            int coinCount = 6 + lvlNumber * 3;
            for (int i = 0; i < coinCount; i++)
            {
                level.Coins.Add(new Coin
                {
                    Position = new Vector2(40 + i * 75, GroundY - 45 - (i % 3) * 35),
                    Type = i % 3 == 0 ? CoinType.Gold : CoinType.Normal
                });
            }

            level.Enemies.Add(new Enemy
            {
                Position = new Vector2(200, GroundY - 49),
                SpawnPosition = new Vector2(200, GroundY - 49),
                PatrolStartX = 180,
                PatrolEndX = 280,
                PatrolSpeed = 60f,
                Active = true,
                MovingRight = true
            });
            level.Enemies.Add(new Enemy
            {
                Position = new Vector2(500, GroundY - 109),
                SpawnPosition = new Vector2(500, GroundY - 109),
                PatrolStartX = 480,
                PatrolEndX = 580,
                PatrolSpeed = 60f,
                Active = true,
                MovingRight = false
            });

            level.Name = $"Level {lvlNumber}";
            level.StartTime = 90;
            level.RequiredCoins = Math.Min(4 + lvlNumber * 2, level.Coins.Count - 1);

            return level;
        }

        private static float Clamp(float value, float min, float max)
        {
            return value < min ? min : (value > max ? max : value);
        }
    }
}