#nullable enable

using Cysharp.Threading.Tasks;

namespace Uft.AdvTools.Commands
{
    public class CmdImageEffect : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.Effect;

        protected bool IsOn { get; set; }
        protected string CameraName { get; set; }
        protected string ImageEffectName { get; set; }
        protected float FadeSeconds { get; set; }
        protected WaitType WaitType { get; set; }

        public CmdImageEffect(bool isOn, string cameraName, string imageEffectName, float? fadeSeconds, WaitType waitType = WaitType.Default)
        {
            this.IsOn = isOn;
            this.CameraName = cameraName;
            this.ImageEffectName = imageEffectName;
            this.FadeSeconds = fadeSeconds ?? 0.0f;
            this.WaitType = waitType;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            var noWait = this.WaitType == WaitType.NoWait;
            if (!noWait) scenarioExecutor.IsWaiting = true;
            UniTask.Void(async () =>
            {
                try
                {
                    await advRoot.PostEffectManager.SetImageEffectAsync(this.ImageEffectName, this.IsOn ? 1.0f : 0.0f, this.FadeSeconds);
                }
                finally
                {
                    if (!noWait) scenarioExecutor.IsWaiting = false;
                }
            });
        }
    }
}
