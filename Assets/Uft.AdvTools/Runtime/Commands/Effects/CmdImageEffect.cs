#nullable enable

using Cysharp.Threading.Tasks;
using Uft.UnityUtils;

namespace Uft.AdvTools.Commands
{
    public class CmdImageEffect : ICommand
    {
        public const string GrayScale = "GrayScale";
        public const string Sepia = "Sepia";

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
                    if (this.ImageEffectName == Sepia)
                    {

                        await advRoot.PostEffectManager.SetSepiaAsync(this.IsOn ? 1.0f : 0.0f, this.FadeSeconds);
                    }
                    else if (this.ImageEffectName == GrayScale)
                    {
                        await advRoot.PostEffectManager.SetGrayScaleAsync(this.IsOn ? 1.0f : 0.0f, this.FadeSeconds);
                    }
                    else
                    {
                        DevLog.LogWarning($"[{nameof(CmdImageEffect)}] Invalid ImageEffect/ImageEffectOff effect name. e.g. the effect name is case-sensitive.");
                    }

                }
                finally
                {
                    scenarioExecutor.IsWaiting = false;
                }
            });
        }
    }
}
