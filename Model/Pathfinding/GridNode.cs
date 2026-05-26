using System;

namespace TimeTax.Model.Pathfinding
{
    public struct GridNode : IEquatable<GridNode>
    {
        public int X;
        public int Y;

        public GridNode(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(GridNode other) => X == other.X && Y == other.Y;
        public override bool Equals(object? obj) => obj is GridNode other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y);
        public static bool operator ==(GridNode a, GridNode b) => a.Equals(b);
        public static bool operator !=(GridNode a, GridNode b) => !a.Equals(b);
    }
}