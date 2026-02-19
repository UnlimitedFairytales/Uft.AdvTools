#nullable enable

using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Uft.AdvTools.Commands
{
    public class CmdFadeIn : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.Effect;

        protected Color FadeColor { get; set; }
        protected string? CameraName { get; set; } // TODO: 対応
        protected string? RuleName { get; set; }
        protected float RuleSoftness { get; set; }
        protected bool IsInvert { get; set; }
        protected float FadeSeconds { get; set; }
        protected WaitType WaitType { get; set; }

        public CmdFadeIn(string? fadeColor, string? cameraName, string? ruleName, float? ruleSoftness, bool? isInvert, float? fadeSeconds, WaitType waitType = WaitType.Default)
        {
            if (!ColorUtility.TryParseHtmlString(fadeColor, out var colorValue))
            {
                colorValue = Color.white;
            }
            this.FadeColor = colorValue;
            this.CameraName = cameraName;
            this.RuleName = ruleName;
            this.RuleSoftness = Mathf.Clamp01(ruleSoftness ?? 0.2f);
            this.IsInvert = isInvert ?? false;
            this.FadeSeconds = fadeSeconds ?? 0.2f;
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
                    if (string.IsNullOrWhiteSpace(this.RuleName))
                    {
                        await advRoot.FadeEffect.StartFadeAsync(false, this.FadeSeconds, this.FadeColor);
                    }
                    else
                    {
                        await advRoot.PostEffectManager.SetRuleFadeAsync(this.RuleName, this.FadeColor, this.RuleSoftness, 0.0f, this.IsInvert, this.FadeSeconds);
                    }
                }
                finally
                {
                    if (!noWait) scenarioExecutor.IsWaiting = false;
                }
            });
        }
    }
}
