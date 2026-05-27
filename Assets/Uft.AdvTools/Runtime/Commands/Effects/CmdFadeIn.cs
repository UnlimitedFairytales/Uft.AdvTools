#nullable enable

using Cysharp.Threading.Tasks;
using Uft.AdvTools.View;
using UnityEngine;

namespace Uft.AdvTools.Commands
{
    public class CmdFadeIn : ICommand
    {
        public CommandCategory CommandCategory { get; } = CommandCategory.Effect;

        public static string WIPE_RIGHT_ = "WipeRight";
        public static string WIPE_LEFT__ = "WipeLeft";
        public static string WIPE_UP____ = "WipeUp";
        public static string WIPE_DOWN__ = "WipeDown";
        public static string WIPE_CENTER = "WipeCenter";

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

            if (this.RuleName == WIPE_RIGHT_)
            {
                this.RuleName = PostEffectManager.FADE_HORIZONTAL;
            }
            else if (this.RuleName == WIPE_LEFT__)
            {
                this.RuleName = PostEffectManager.FADE_HORIZONTAL;
                this.IsInvert = !this.IsInvert;
            }
            else if (this.RuleName == WIPE_UP____)
            {
                this.RuleName = PostEffectManager.FADE_VERTICAL;
                this.IsInvert = !this.IsInvert;
            }
            else if (this.RuleName == WIPE_DOWN__)
            {
                this.RuleName = PostEffectManager.FADE_VERTICAL;
            }
            else if (this.RuleName == WIPE_CENTER)
            {
                this.RuleName = PostEffectManager.MOSES_V;
                this.IsInvert = !this.IsInvert;
            }
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
                        if (advRoot.PostEffectManager.RuleFadeIsVisible())
                        {
                            // HACK: 通常フェード表示、ルールフェード非表示、1フレーム
                            await advRoot.FadeEffect.StartFadeAsync(advRoot.destroyCancellationToken, true, 0, this.FadeColor);
                            await advRoot.PostEffectManager.SetRuleFadeAsync(null, this.FadeColor, 0, 0, false, 0);
                            await UniTask.NextFrame();
                        }
                        await advRoot.FadeEffect.StartFadeAsync(advRoot.destroyCancellationToken, false, this.FadeSeconds, this.FadeColor);
                    }
                    else
                    {
                        if (advRoot.FadeEffect.IsOn)
                        {
                            // HACK: ルールフェード表示、通常フェード非表示、1フレーム
                            await advRoot.PostEffectManager.SetRuleFadeAsync(null, this.FadeColor, 0, 1, false, 0);
                            await advRoot.FadeEffect.StartFadeAsync(advRoot.destroyCancellationToken, false, 0, this.FadeColor);
                            await UniTask.NextFrame();
                        }
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
