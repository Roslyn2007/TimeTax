using System;

namespace TimeTax.Model
{
    public class TimeManager
    {
        public float CurrentTime { get; private set; }
        public float StartTime { get; }

        public event Action<float> TimeChanged;
        public event Action<string> ScreenEffectChanged;
        public event Action TimeRanOut;

        public TimeManager(float startTime)
        {
            CurrentTime = startTime;
            StartTime = startTime;
        }

        public void Update(float deltaTime)
        {
            CurrentTime -= deltaTime;
            TimeChanged?.Invoke(CurrentTime);

            if (CurrentTime > 60f)
                ScreenEffectChanged?.Invoke("normal");
            else if (CurrentTime > 30f)
                ScreenEffectChanged?.Invoke("orange");
            else if (CurrentTime > 10f)
                ScreenEffectChanged?.Invoke("red");
            else if (CurrentTime > 0f)
                ScreenEffectChanged?.Invoke("critical");

            if (CurrentTime <= 0f)
            {
                CurrentTime = 0f;
                TimeRanOut?.Invoke();
            }
        }

        public void AddSeconds(float seconds)
        {
            CurrentTime += seconds;
            TimeChanged?.Invoke(CurrentTime);
        }

        public void SubtractSeconds(float seconds)
        {
            CurrentTime -= seconds;
            if (CurrentTime < 0) CurrentTime = 0;
            TimeChanged?.Invoke(CurrentTime);
        }
    }
}