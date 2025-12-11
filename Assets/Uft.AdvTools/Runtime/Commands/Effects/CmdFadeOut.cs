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
        protected string? RuleName { get; set; }
        protected float RuleSoftness { get; set; }
        protected float FadeSeconds { get; set; }

        public CmdFadeOut(string? fadeColor, string? cameraName, string? ruleName, float? ruleSoftness, float? fadeSeconds)
        {
            if (!ColorUtility.TryParseHtmlString(fadeColor, out var colorValue))
            {
                colorValue = Color.white;
            }
            this.FadeColor = colorValue;
            this.CameraName = cameraName;
            this.RuleName = ruleName;
            this.RuleSoftness = Mathf.Clamp01(ruleSoftness ?? 0.2f);
            this.FadeSeconds = fadeSeconds ?? 0.2f;
        }

        public virtual void Run(ScenarioExecutor scenarioExecutor, AdvRoot advRoot)
        {
            scenarioExecutor.IsWaiting = true;
            UniTask.Void(async () =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(this.RuleName))
                    {
                        await advRoot.FadeEffect.StartFadeAsync(true, this.FadeSeconds, this.FadeColor);
                    }
                    else
                    {
                        await advRoot.PostEffectManager.SetRuleFadeAsync(this.RuleName, this.FadeColor, this.RuleSoftness, 1.0f, false, this.FadeSeconds);
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
