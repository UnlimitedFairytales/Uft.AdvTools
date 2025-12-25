#nullable enable

namespace Uft.AdvTools
{
    public class AutoNext
    {
        public const float DEFAULT_ADJUST_WEIGHT = 0.07f;

        protected float _counter = 0;
        protected float _baseTime = 2.0f;
        protected float _adjustTime = 0.0f;
        public void SetIsAutoNextReadyTimeAdjust(int textLength, float perAdjust = DEFAULT_ADJUST_WEIGHT) => this._adjustTime = textLength * perAdjust;
        public virtual bool IsReady => (this._baseTime + this._adjustTime) <= this._counter;

        public void ClearCounter()
        {
            this._counter = 0;
        }

        public void UpdateFrame(bool isCountable, float deltaTime)
        {
            if (isCountable)
            {
                this._counter += deltaTime;
            }
            else
            {
                this._counter = 0;
            }
        }
    }
}
