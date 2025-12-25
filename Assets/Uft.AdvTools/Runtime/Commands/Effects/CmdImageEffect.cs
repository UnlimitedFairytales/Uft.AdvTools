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

        public CmdImageEffect(bool isOn, string cameraName, string imageEffectName, float? fadeSeconds)
        {
            this.IsOn = isOn;
            this.CameraName = cameraName;
            this.ImageEffectName = imageEffectName;
            this.FadeSeconds = fadeSeconds ?? 0.0f;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            scenarioExecutor.IsWaiting = true;
            UniTask.Void(async () =>
            {
                try
                {
                    await advRoot.PostEffectManager.SetImageEffectAsync(this.ImageEffectName, this.IsOn ? 1.0f : 0.0f, this.FadeSeconds);
                }
                finally
                {
                    scenarioExecutor.IsWaiting = false;
                }
            });
        }
    }
}
