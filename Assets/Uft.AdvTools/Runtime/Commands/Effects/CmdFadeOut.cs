#nullable enable

using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Uft.AdvTools.Commands
{
    public class CmdFadeOut : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.Effect;

        protected Color FadeColor { get; set; }
        protected string? CameraName { get; set; } // TODO: 対応
        protected float FadeSeconds { get; set; }

        public CmdFadeOut(string? fadeColor, string? cameraName, float? fadeSeconds)
        {
            if (!ColorUtility.TryParseHtmlString(fadeColor, out var colorValue))
            {
                colorValue = Color.white;
            }
            this.FadeColor = colorValue;
            this.CameraName = cameraName;
            this.FadeSeconds = fadeSeconds ?? 0.2f;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            scenarioExecutor.IsWaiting = true;
            UniTask.Void(async () =>
            {
                try
                {
                    await advRoot.FadeEffect.StartFadeAsync(true, this.FadeSeconds, this.FadeColor);
                }
                finally
                {
                    scenarioExecutor.IsWaiting = false;
                }
            });
        }
    }
}
