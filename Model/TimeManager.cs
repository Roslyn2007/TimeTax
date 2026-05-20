using System;

namespace TimeTax.Model
{
    public class TimeManager
    {
        public float CurrentTime { get; private set; }
        public float StartTime { get; }

        public event Action<float>? TimeChanged;
        public event Action<string>? ScreenEffectChanged;
        public event Action? TimeRanOut;

        private float previousTime = -1f;

        public TimeManager(float startTime)
        {
            CurrentTime = startTime;
            StartTime = startTime;
            previousTime = startTime;
        }

        public void Update(float deltaTime)
        {
            if (deltaTime < 0) deltaTime = 0;
            if (deltaTime > 0.1f) deltaTime = 0.1f;

            CurrentTime -= deltaTime;
            if (CurrentTime < 0) CurrentTime = 0;

            // ВСЕГДА вызываем событие — таймер должен обновляться каждый кадр
            TimeChanged?.Invoke(CurrentTime);

            string effect = CurrentTime switch
            {
                > 60f => "normal",
                > 30f => "orange",
                > 10f => "red",
                > 0f => "critical",
                _ => "dead"
            };

            ScreenEffectChanged?.Invoke(effect);

            if (CurrentTime <= 0f && previousTime > 0f)
            {
                CurrentTime = 0f;
                TimeRanOut?.Invoke();
            }

            previousTime = CurrentTime;
        }

        public void AddSeconds(float seconds)
        {
            CurrentTime += seconds;
            if (CurrentTime < 0) CurrentTime = 0;
            TimeChanged?.Invoke(CurrentTime);
            previousTime = CurrentTime;
        }

        public void SubtractSeconds(float seconds)
        {
            CurrentTime -= seconds;
            if (CurrentTime < 0) CurrentTime = 0;
            TimeChanged?.Invoke(CurrentTime);
            previousTime = CurrentTime;
        }
    }
}