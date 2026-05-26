using TimeTax.Model;
using System;
using System.Collections.Generic;

namespace TimeTax.Model.Pathfinding
{
    public class AStarPathfinder
    {
        private const float GridSize = 20f;
        private int gridWidth;
        private int gridHeight;
        private bool[,] walkableGrid = new bool[0, 0];
        private Level level;

        public AStarPathfinder(Level level)
        {
            this.level = level;
            gridWidth = (int)Math.Ceiling(800f / GridSize);
            gridHeight = (int)Math.Ceiling(480f / GridSize);
            BuildWalkableGrid();
        }

        public void RebuildWalkableGrid()
        {
            BuildWalkableGrid();
        }

        private void BuildWalkableGrid()
        {
            walkableGrid = new bool[gridWidth, gridHeight];
            
            for (int x = 0; x < gridWidth; x++)
                for (int y = 0; y < gridHeight; y++)
                    walkableGrid[x, y] = true;

            foreach (var platform in level.Platforms)
                MarkRectAsBlocked(platform.Position, platform.Width, platform.Height);

            foreach (var fp in level.FadingPlatforms)
            {
                if (fp.IsVisible)
                    MarkRectAsBlocked(fp.Position, fp.Width, fp.Height);
            }

            for (int x = 0; x < gridWidth; x++)
            {
                walkableGrid[x, 0] = false;
                walkableGrid[x, gridHeight - 1] = false;
            }
            for (int y = 0; y < gridHeight; y++)
            {
                walkableGrid[0, y] = false;
                walkableGrid[gridWidth - 1, y] = false;
            }
        }

        private void MarkRectAsBlocked(Vector2 pos, float width, float height)
        {
            int startX = Math.Max(0, (int)(pos.X / GridSize));
            int endX = Math.Min(gridWidth - 1, (int)((pos.X + width) / GridSize));
            int startY = Math.Max(0, (int)(pos.Y / GridSize));
            int endY = Math.Min(gridHeight - 1, (int)((pos.Y + height) / GridSize));

            for (int x = startX; x <= endX; x++)
                for (int y = startY; y <= endY; y++)
                    walkableGrid[x, y] = false;
        }

        public List<Vector2>? FindPath(Vector2 start, Vector2 goal)
        {
            var startNode = WorldToGrid(start);
            var goalNode = WorldToGrid(goal);

            if (!IsValid(startNode) || !IsValid(goalNode))
                return null;

            if (!walkableGrid[startNode.X, startNode.Y])
            {
                var maybeStart = FindNearestWalkable(startNode);
                if (maybeStart == null) return null;
                startNode = maybeStart.Value;
            }
            if (!walkableGrid[goalNode.X, goalNode.Y])
            {
                var maybeGoal = FindNearestWalkable(goalNode);
                if (maybeGoal == null) return null;
                goalNode = maybeGoal.Value;
            }

            var openSet = new PriorityQueue<GridNode, float>();
            var cameFrom = new Dictionary<GridNode, GridNode>();
            var gScore = new Dictionary<GridNode, float>();
            var closedSet = new HashSet<GridNode>();

            gScore[startNode] = 0;
            float startF = Heuristic(startNode, goalNode);
            openSet.Enqueue(startNode, startF);

            while (openSet.Count > 0)
            {
                var current = openSet.Dequeue();

                if (current.Equals(goalNode))
                    return ReconstructPath(cameFrom, current);

                closedSet.Add(current);

                foreach (var neighbor in GetNeighbors(current))
                {
                    if (closedSet.Contains(neighbor))
                        continue;

                    float tentativeG = gScore[current] + Distance(current, neighbor);

                    if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeG;
                        float fScore = tentativeG + Heuristic(neighbor, goalNode);
                        openSet.Enqueue(neighbor, fScore);
                    }
                }
            }

            return null;
        }

        private GridNode? FindNearestWalkable(GridNode node)
        {
            var queue = new Queue<GridNode>();
            var visited = new HashSet<GridNode>();
            queue.Enqueue(node);
            visited.Add(node);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (IsValid(current) && walkableGrid[current.X, current.Y])
                    return current;

                foreach (var neighbor in GetNeighbors(current))
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return null;
        }

        private List<Vector2> ReconstructPath(Dictionary<GridNode, GridNode> cameFrom, GridNode current)
        {
            var path = new List<Vector2> { GridToWorld(current) };
            
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Add(GridToWorld(current));
            }
            
            path.Reverse();
            return SimplifyPath(path);
        }

        private List<Vector2> SimplifyPath(List<Vector2> path)
        {
            if (path.Count <= 2) return path;

            var simplified = new List<Vector2> { path[0] };
            int i = 0;

            while (i < path.Count - 1)
            {
                int furthest = i + 1;
                
                for (int j = i + 2; j < path.Count; j++)
                {
                    if (HasLineOfSight(path[i], path[j]))
                        furthest = j;
                }
                
                simplified.Add(path[furthest]);
                i = furthest;
            }

            return simplified;
        }

        private bool HasLineOfSight(Vector2 from, Vector2 to)
        {
            float steps = Vector2.Distance(from, to) / GridSize;
            for (int i = 0; i <= steps; i++)
            {
                float t = i / steps;
                var point = new Vector2(
                    from.X + (to.X - from.X) * t,
                    from.Y + (to.Y - from.Y) * t
                );
                var node = WorldToGrid(point);
                if (IsValid(node) && !walkableGrid[node.X, node.Y])
                    return false;
            }
            return true;
        }

        private List<GridNode> GetNeighbors(GridNode node)
        {
            var neighbors = new List<GridNode>();
            int[] dx = { -1, 1, 0, 0, -1, -1, 1, 1 };
            int[] dy = { 0, 0, -1, 1, -1, 1, -1, 1 };

            for (int i = 0; i < 8; i++)
            {
                var neighbor = new GridNode(node.X + dx[i], node.Y + dy[i]);
                if (IsValid(neighbor) && walkableGrid[neighbor.X, neighbor.Y])
                {
                    if (i >= 4)
                    {
                        var check1 = new GridNode(node.X + dx[i], node.Y);
                        var check2 = new GridNode(node.X, node.Y + dy[i]);
                        if (!walkableGrid[check1.X, check1.Y] && !walkableGrid[check2.X, check2.Y])
                            continue;
                    }
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        private bool IsValid(GridNode node)
        {
            return node.X >= 0 && node.X < gridWidth && node.Y >= 0 && node.Y < gridHeight;
        }

        private float Heuristic(GridNode a, GridNode b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            return MathF.Sqrt(dx * dx + dy * dy) * GridSize;
        }

        private float Distance(GridNode a, GridNode b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            return MathF.Sqrt(dx * dx + dy * dy) * GridSize;
        }

        private GridNode WorldToGrid(Vector2 world)
        {
            return new GridNode(
                (int)(world.X / GridSize),
                (int)(world.Y / GridSize)
            );
        }

        private Vector2 GridToWorld(GridNode grid)
        {
            return new Vector2(
                grid.X * GridSize + GridSize / 2,
                grid.Y * GridSize + GridSize / 2
            );
        }
    }
}