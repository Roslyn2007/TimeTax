using System.Collections.Generic;
using TimeTax.Model.Entities;

namespace TimeTax.Model.Collision
{
    public class QuadTree
    {
        private const int MaxObjects = 10;
        private const int MaxLevels = 5;
        
        private int level;
        private List<ICollidable> objects;
        private Rect bounds;
        private QuadTree?[] nodes;
        private bool isSplit;

        public QuadTree(int level, Rect bounds)
        {
            this.level = level;
            this.bounds = bounds;
            this.objects = new List<ICollidable>();
            this.nodes = new QuadTree?[4];
            this.isSplit = false;
        }

        public void Clear()
        {
            objects.Clear();
            
            if (isSplit)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (nodes[i] != null)
                    {
                        nodes[i]!.Clear();
                        nodes[i] = null;
                    }
                }
                isSplit = false;
            }
        }

        private void Split()
        {
            int subWidth = bounds.Width / 2;
            int subHeight = bounds.Height / 2;
            int x = bounds.X;
            int y = bounds.Y;

            nodes[0] = new QuadTree(level + 1, new Rect(x, y, subWidth, subHeight));
            nodes[1] = new QuadTree(level + 1, new Rect(x + subWidth, y, subWidth, subHeight));
            nodes[2] = new QuadTree(level + 1, new Rect(x, y + subHeight, subWidth, subHeight));
            nodes[3] = new QuadTree(level + 1, new Rect(x + subWidth, y + subHeight, subWidth, subHeight));
            
            isSplit = true;
        }

        private int GetIndex(Rect rect)
        {
            int index = -1;
            double verticalMidpoint = bounds.X + bounds.Width / 2.0;
            double horizontalMidpoint = bounds.Y + bounds.Height / 2.0;

            bool topQuadrant = rect.Y < horizontalMidpoint && rect.Y + rect.Height < horizontalMidpoint;
            bool bottomQuadrant = rect.Y > horizontalMidpoint;

            if (rect.X < verticalMidpoint && rect.X + rect.Width < verticalMidpoint)
            {
                if (topQuadrant) index = 0;
                else if (bottomQuadrant) index = 2;
            }
            else if (rect.X > verticalMidpoint)
            {
                if (topQuadrant) index = 1;
                else if (bottomQuadrant) index = 3;
            }

            return index;
        }

        public void Insert(ICollidable obj)
        {
            var objBounds = GetObjectBounds(obj);

            if (isSplit)
            {
                int index = GetIndex(objBounds);
                if (index != -1)
                {
                    nodes[index]!.Insert(obj);
                    return;
                }
            }

            objects.Add(obj);

            if (objects.Count > MaxObjects && level < MaxLevels && !isSplit)
            {
                Split();
                
                int i = 0;
                while (i < objects.Count)
                {
                    int index = GetIndex(GetObjectBounds(objects[i]));
                    if (index != -1)
                    {
                        nodes[index]!.Insert(objects[i]);
                        objects.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }

        public List<ICollidable> Retrieve(List<ICollidable> returnObjects, Rect area)
        {
            int index = GetIndex(area);
            
            if (index != -1 && isSplit)
                nodes[index]!.Retrieve(returnObjects, area);

            returnObjects.AddRange(objects);
            return returnObjects;
        }

        public List<ICollidable> RetrievePotentialCollisions(ICollidable obj)
        {
            var result = new List<ICollidable>();
            var bounds = GetObjectBounds(obj);
            return Retrieve(result, bounds);
        }

        private Rect GetObjectBounds(ICollidable obj)
        {
            var b = obj.GetBounds();
            return new Rect((int)b.left, (int)b.top, (int)(b.right - b.left), (int)(b.bottom - b.top));
        }
    }
}