using System;
using System.Collections.Generic;
using System.Diagnostics;
using TimeTax.Model;
using TimeTax.Model.Pathfinding;

namespace TimeTax.Model.Entities
{
    public class SmartEnemy : Enemy
    {
        private AStarPathfinder? pathfinder;
        private List<Vector2>? currentPath;
        private int pathIndex;

        public EnemyState CurrentState { get; private set; } = EnemyState.Patrol;

        public float ViewRadius { get; set; } = 150f;
        public float ViewAngle { get; set; } = 120f;
        public float ChaseSpeed { get; set; } = 120f;

        private Vector2 lastKnownPlayerPos = Vector2.Zero;
        private float searchTimer;
        private const float SearchDuration = 3f;
        private const float ChaseMemoryDuration = 2.0f;
        private float chaseMemoryTimer = 0f;

        private Vector2 patrolStart = Vector2.Zero;
        private Vector2 patrolEnd = Vector2.Zero;
        private bool wasPatrollingRight;
        private Level? level;

        private const float JumpVelocity = -350f;

        private float stuckTimer = 0f;
        private float lastX = 0f;

        private float aiTick = 0f;
        private const float AiTickRate = 0.2f;
        private bool doLogic = false;
        private bool cachedLOS = false;

        private float pathRecalcTimer = 0f;
        private const float PathRecalcInterval = 1.0f;
        private Vector2 lastPathTarget = new Vector2(float.MaxValue, float.MaxValue);
        private const float PathTargetThreshold = 60f;

        private Vector2 _playerPos = Vector2.Zero;
        private bool _playerAlive;

        public void InitializeAI(AStarPathfinder pathfinder, float patrolStartX, float patrolEndX, Level level)
        {
            this.pathfinder = pathfinder;
            this.level = level;
            this.PatrolStartX = patrolStartX;
            this.PatrolEndX = patrolEndX;
            this.patrolStart = new Vector2(patrolStartX, SpawnPosition.Y);
            this.patrolEnd = new Vector2(patrolEndX, SpawnPosition.Y);
            this.wasPatrollingRight = MovingRight;
            this.lastX = Position.X;
            this.aiTick = 0f;
        }

        public override void Respawn()
        {
            base.Respawn();
            CurrentState = EnemyState.Patrol;
            currentPath = null;
            pathIndex = 0;
            chaseMemoryTimer = 0f;
            searchTimer = 0f;
            stuckTimer = 0f;
            lastX = SpawnPosition.X;
            pathRecalcTimer = 0f;
            lastPathTarget = new Vector2(float.MaxValue, float.MaxValue);
            patrolStart = new Vector2(PatrolStartX, SpawnPosition.Y);
            patrolEnd = new Vector2(PatrolEndX, SpawnPosition.Y);
            wasPatrollingRight = true;
            aiTick = 0f;
        }

        public override void Update(float deltaTime)
        {
            if (!Active) return;

            bool fellOff = Position.Y + Height >= 480f || Position.Y > 500f || Position.Y < -100 || Position.X < -50 || Position.X > 850;

            if (fellOff)
            {
                Debug.WriteLine($"[SmartEnemy] Respawning at {Position.X:F1},{Position.Y:F1} -> {SpawnPosition.X:F1},{SpawnPosition.Y:F1}");
                Respawn();
                return;
            }

            aiTick -= deltaTime;
            doLogic = aiTick <= 0f;
            if (doLogic) aiTick = AiTickRate;

            velocity.Y += Gravity * deltaTime;
            isGrounded = false;

            if (doLogic)
            {
                switch (CurrentState)
                {
                    case EnemyState.Patrol:
                        UpdatePatrolLogic();
                        break;
                    case EnemyState.Chase:
                        UpdateChaseLogic();
                        break;
                    case EnemyState.Search:
                        UpdateSearchLogic();
                        break;
                    case EnemyState.Return:
                        UpdateReturnLogic();
                        break;
                }
            }

            if (CurrentState == EnemyState.Search || CurrentState == EnemyState.Return)
                MoveAlongPath(deltaTime, CurrentState == EnemyState.Search ? PatrolSpeed * 0.7f : PatrolSpeed);

            Position += velocity * deltaTime;
            base.ResolveCollisions();

            if (MathF.Abs(velocity.X) > 10f && isGrounded)
            {
                if (MathF.Abs(Position.X - lastX) < 0.5f)
                {
                    stuckTimer += deltaTime;
                    if (stuckTimer > 0.4f)
                    {
                        velocity.Y = JumpVelocity;
                        isGrounded = false;
                        stuckTimer = 0f;
                    }
                }
                else stuckTimer = 0f;
            }
            else stuckTimer = 0f;
            lastX = Position.X;
        }

        public void UpdateAI(float deltaTime, Vector2 playerPos, bool playerAlive)
        {
            _playerPos = playerPos;
            _playerAlive = playerAlive;
        }

        private void UpdatePatrolLogic()
        {
            if (_playerAlive && CanSeePlayerCached(_playerPos))
            {
                TransitionToChase(_playerPos);
                return;
            }

            if (Position.X < 0)
            {
                Position = new Vector2(0, Position.Y);
                velocity.X = Math.Abs(velocity.X);
                MovingRight = true;
                return;
            }
            if (Position.X + Width > 800)
            {
                Position = new Vector2(800 - Width, Position.Y);
                velocity.X = -Math.Abs(velocity.X);
                MovingRight = false;
                return;
            }

            float targetX = MovingRight ? PatrolEndX : PatrolStartX;
            float dirX = targetX > Position.X ? 1 : -1;

            if (MovingRight && Position.X >= PatrolEndX - 2)
            {
                Position = new Vector2(PatrolEndX, Position.Y);
                MovingRight = false;
                wasPatrollingRight = false;
                velocity.X = -PatrolSpeed;
            }
            else if (!MovingRight && Position.X <= PatrolStartX + 2)
            {
                Position = new Vector2(PatrolStartX, Position.Y);
                MovingRight = true;
                wasPatrollingRight = true;
                velocity.X = PatrolSpeed;
            }
            else
            {
                velocity.X = dirX * PatrolSpeed;
            }
        }

        private void UpdateChaseLogic()
        {
            bool canSee = _playerAlive && CanSeePlayerCached(_playerPos);

            if (canSee)
            {
                chaseMemoryTimer = ChaseMemoryDuration;
                lastKnownPlayerPos = _playerPos;
            }
            else
            {
                chaseMemoryTimer -= AiTickRate;
            }

            if (chaseMemoryTimer > 0 && _playerAlive)
            {
                MoveTowardsPlayer(lastKnownPlayerPos);
            }
            else
            {
                TransitionToSearch();
            }
        }

        private void UpdateSearchLogic()
        {
            searchTimer -= AiTickRate;
            EnsurePath(lastKnownPlayerPos, force: false);

            if (currentPath == null || pathIndex >= currentPath.Count)
            {
                float dx = lastKnownPlayerPos.X - Position.X;
                if (MathF.Abs(dx) > 2f)
                {
                    velocity.X = (dx > 0 ? 1 : -1) * PatrolSpeed * 0.7f;
                    MovingRight = dx > 0;
                }
                else velocity.X = 0;
            }

            if (searchTimer <= 0)
                TransitionToReturn();
        }

        private void UpdateReturnLogic()
        {
            Vector2 returnTarget = wasPatrollingRight ? patrolStart : patrolEnd;
            returnTarget.Y = Position.Y;

            EnsurePath(returnTarget, force: false);

            if (currentPath == null || pathIndex >= currentPath.Count)
            {
                float dirX = returnTarget.X > Position.X ? 1 : -1;
                velocity.X = dirX * PatrolSpeed;
            }

            if (MathF.Abs(Position.X - returnTarget.X) < 15f)
            {
                patrolStart.Y = Position.Y;
                patrolEnd.Y = Position.Y;
                PatrolStartX = patrolStart.X;
                PatrolEndX = patrolEnd.X;

                CurrentState = EnemyState.Patrol;
                MovingRight = wasPatrollingRight;
                velocity.X = 0;
                currentPath = null;
                pathIndex = 0;
            }
        }

        private void MoveTowardsPlayer(Vector2 playerPos)
        {
            float dx = playerPos.X - Position.X;
            float dy = playerPos.Y - Position.Y;

            if (MathF.Abs(dx) < 8f) velocity.X = 0;
            else if (dx > 8f) { velocity.X = ChaseSpeed; MovingRight = true; }
            else if (dx < -8f) { velocity.X = -ChaseSpeed; MovingRight = false; }

            if (isGrounded && dy < -25f && MathF.Abs(dx) < 150f)
            {
                velocity.Y = JumpVelocity;
                isGrounded = false;
            }
        }

        private void EnsurePath(Vector2 target, bool force)
        {
            pathRecalcTimer -= AiTickRate;

            bool needsRecalc = force
                || currentPath == null
                || pathIndex >= currentPath.Count
                || pathRecalcTimer <= 0
                || Vector2.Distance(target, lastPathTarget) > PathTargetThreshold;

            if (!needsRecalc) return;

            currentPath = pathfinder?.FindPath(Position, target);
            pathIndex = 0;
            lastPathTarget = target;
            pathRecalcTimer = PathRecalcInterval;
        }

        private bool CanSeePlayerCached(Vector2 playerPos)
        {
            float distance = Vector2.Distance(Position, playerPos);
            if (distance > ViewRadius) return false;
            if (distance < 35f) return HasLineOfSightFast(playerPos);

            if (!IsInViewAngle(playerPos)) return false;

            cachedLOS = HasLineOfSightFast(playerPos);
            return cachedLOS;
        }

        private bool IsInViewAngle(Vector2 playerPos)
        {
            Vector2 toPlayer = playerPos - Position;
            float angleToPlayer = MathF.Atan2(toPlayer.Y, toPlayer.X) * 180f / MathF.PI;
            float facingAngle = MovingRight ? 0f : 180f;
            float angleDiff = MathF.Abs(NormalizeAngle(angleToPlayer - facingAngle));
            return angleDiff <= ViewAngle / 2f;
        }

        private bool HasLineOfSightFast(Vector2 target)
        {
            if (level == null) return true;

            float myX = Position.X + Width / 2;
            float myY = Position.Y + Height / 2;

            float rayMinX = Math.Min(myX, target.X) - 10;
            float rayMaxX = Math.Max(myX, target.X) + 10;
            float rayMinY = Math.Min(myY, target.Y) - 10;
            float rayMaxY = Math.Max(myY, target.Y) + 10;

            Vector2 start = new Vector2(myX, myY);
            Vector2 end = target;

            foreach (var platform in level.Platforms)
            {
                var p = platform.GetBounds();
                if (p.right < rayMinX || p.left > rayMaxX || p.bottom < rayMinY || p.top > rayMaxY)
                    continue;
                if (LineIntersectsRect(start, end, p))
                    return false;
            }

            foreach (var fp in level.FadingPlatforms)
            {
                if (!fp.IsVisible) continue;
                var p = fp.GetBounds();
                if (p.right < rayMinX || p.left > rayMaxX || p.bottom < rayMinY || p.top > rayMaxY)
                    continue;
                if (LineIntersectsRect(start, end, p))
                    return false;
            }

            return true;
        }

        private void MoveAlongPath(float deltaTime, float speed)
        {
            if (currentPath == null || pathIndex >= currentPath.Count) return;

            while (pathIndex < currentPath.Count)
            {
                Vector2 target = currentPath[pathIndex];
                Vector2 direction = target - Position;
                float distance = Vector2.Distance(Position, target);

                if (distance < 5f)
                {
                    pathIndex++;
                    continue;
                }

                if (MathF.Abs(direction.X) < 2f)
                {
                    if (direction.Y < -10f && isGrounded)
                    {
                        velocity.Y = JumpVelocity;
                        isGrounded = false;
                    }
                    pathIndex++;
                    continue;
                }

                if (isGrounded && direction.Y < -20f && MathF.Abs(direction.X) < 80f)
                {
                    velocity.Y = JumpVelocity;
                    isGrounded = false;
                }

                float dirX = direction.X > 0 ? 1 : -1;
                velocity.X = dirX * speed;
                MovingRight = dirX > 0;
                return;
            }
        }

        private void TransitionToChase(Vector2 playerPos)
        {
            CurrentState = EnemyState.Chase;
            lastKnownPlayerPos = playerPos;
            chaseMemoryTimer = ChaseMemoryDuration;
            currentPath = null;
            pathIndex = 0;
            pathRecalcTimer = 0f;
        }

        private void TransitionToSearch()
        {
            CurrentState = EnemyState.Search;
            searchTimer = SearchDuration;
            currentPath = null;
            velocity.X = 0;
            pathRecalcTimer = 0f;
        }

        private void TransitionToReturn()
        {
            CurrentState = EnemyState.Return;
            currentPath = null;
            velocity.X = 0;
            pathRecalcTimer = 0f;
        }

        private bool LineIntersectsRect(Vector2 p1, Vector2 p2, (float left, float right, float top, float bottom) rect)
        {
            float t0 = 0f, t1 = 1f;
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;

            (float p, float q)[] edges = new (float, float)[]
            {
                (-dx, p1.X - rect.left),
                ( dx, rect.right - p1.X),
                (-dy, p1.Y - rect.top),
                ( dy, rect.bottom - p1.Y)
            };

            foreach (var (p, q) in edges)
            {
                if (p == 0) { if (q < 0) return false; }
                else
                {
                    float r = q / p;
                    if (p < 0) { if (r > t1) return false; if (r > t0) t0 = r; }
                    else       { if (r < t0) return false; if (r < t1) t1 = r; }
                }
            }
            return true;
        }

        private float NormalizeAngle(float angle)
        {
            while (angle > 180f) angle -= 360f;
            while (angle < -180f) angle += 360f;
            return angle;
        }
    }
}