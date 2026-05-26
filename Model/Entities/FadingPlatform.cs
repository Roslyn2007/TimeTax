using System;

namespace TimeTax.Model.Entities
{
    public class FadingPlatform : Entity
    {
        public bool IsVisible { get; set; } = true;
        public float FadeTimer { get; set; } = 0f;
        public float ReappearTimer { get; set; } = 0f;

        public const float FadeDuration = 2f;
        public const float VisibleDuration = 5f;

        public event Action<FadingPlatform>? VisibilityChanged;

        public override (float left, float right, float top, float bottom) GetBounds()
        {
            if (!IsVisible)
                return (0, 0, 0, 0);

            return base.GetBounds();
        }

        public void Update(float deltaTime)
        {
            if (IsVisible)
            {
                FadeTimer += deltaTime;
                if (FadeTimer >= VisibleDuration)
                {
                    IsVisible = false;
                    FadeTimer = 0f;
                    VisibilityChanged?.Invoke(this);
                }
            }
            else
            {
                ReappearTimer += deltaTime;
                if (ReappearTimer >= FadeDuration)
                {
                    IsVisible = true;
                    ReappearTimer = 0f;
                    VisibilityChanged?.Invoke(this);
                }
            }
        }
    }
}