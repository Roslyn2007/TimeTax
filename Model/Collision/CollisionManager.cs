using System.Collections.Generic;
using TimeTax.Model.Entities;

namespace TimeTax.Model.Collision
{
    public class CollisionManager
    {
        private QuadTree quadTree;
        private Rect worldBounds;

        public CollisionManager(float worldWidth, float worldHeight)
        {
            worldBounds = new Rect(0, 0, (int)worldWidth, (int)worldHeight);
            quadTree = new QuadTree(0, worldBounds);
        }

        public void RebuildQuadTree(List<ICollidable> staticObjects)
        {
            quadTree.Clear();
            foreach (var obj in staticObjects)
            {
                quadTree.Insert(obj);
            }
        }

        public List<ICollidable> GetPotentialCollisions(ICollidable obj)
        {
            return quadTree.RetrievePotentialCollisions(obj);
        }

        public bool CheckCollision(ICollidable a, ICollidable b)
        {
            var ab = a.GetBounds();
            var bb = b.GetBounds();
            return AABBIntersect(ab, bb);
        }

        private bool AABBIntersect((float left, float right, float top, float bottom) a, 
                                   (float left, float right, float top, float bottom) b)
        {
            return a.left < b.right && a.right > b.left && 
                   a.top < b.bottom && a.bottom > b.top;
        }
    }
}