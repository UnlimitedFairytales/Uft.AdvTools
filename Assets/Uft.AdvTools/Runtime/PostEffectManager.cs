#nullable enable

using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

namespace Uft.AdvTools
{
    public class PostEffectManager : MonoBehaviour
    {
        const string DIRECTIONAL_GHOST = "DIRECTIONAL_GHOST";
        const string GRAY_SCALE = "GRAY_SCALE";
        const string SEPIA = "SEPIA";
        const string RULE = "RULE";

        const string FADE_HORIZONTAL = "FadeHorizontal";
        const string CLOUD = "Cloud";

        [SerializeField] Texture? _texFadeHorizontal;
        [SerializeField] Texture? _texCloud;


        AdvRoot? _advRootRef;

        Tweener? _directionalGhostTweener;
        Tweener? _grayScaleTweener;
        Tweener? _sepiaTweener;
        Tweener? _ruleFadeTweener;

        public void Setup(AdvRoot advRootRef)
        {
            this._advRootRef = advRootRef;
        }

        public async UniTask SetDirectionalGhostAsync(float endValue, float fadeSeconds, bool completesPrevious = false)
        {
            if (this._advRootRef == null) return;

            await this.SetEffectAsync(
                DIRECTIONAL_GHOST,
                () => this._advRootRef.WideCameraDirectionalGhostPostEffect.Amount,
                x => this._advRootRef.WideCameraDirectionalGhostPostEffect.Amount = x,
                endValue,
                fadeSeconds,
                completesPrevious);
        }

        public async UniTask SetGrayScaleAsync(float endValue, float fadeSeconds, bool completesPrevious = false)
        {
            if (this._advRootRef == null) return;

            await this.SetEffectAsync(
                GRAY_SCALE,
                () => this._advRootRef.WideCameraGrayscalePostEffect.Amount,
                x => this._advRootRef.WideCameraGrayscalePostEffect.Amount = x,
                endValue,
                fadeSeconds,
                completesPrevious);
        }

        public async UniTask SetSepiaAsync(float endValue, float fadeSeconds, bool completesPrevious = false)
        {
            if (this._advRootRef == null) return;

            await this.SetEffectAsync(
                SEPIA,
                () => this._advRootRef.WideCameraSepiaPostEffect.Amount,
                x => this._advRootRef.WideCameraSepiaPostEffect.Amount = x,
                endValue,
                fadeSeconds,
                completesPrevious);
        }

        public async UniTask SetRuleFadeAsync(string ruleName, Color color, float ruleSoftness, float endValue, bool isInvert, float fadeSeconds, bool completesPrevious = false)
        {
            if (this._advRootRef == null) return;

            Texture? rule =
                ruleName == FADE_HORIZONTAL ? this._texFadeHorizontal :
                ruleName == CLOUD ? this._texCloud :
                null;
            if (rule == null) return;

            var effectConfig = this._advRootRef.WideCameraRuleFadePostEffect;
            effectConfig.RuleTex = rule;
            effectConfig.SubTexColor = color;
            effectConfig.Softness = ruleSoftness;
            effectConfig.Invert = isInvert ? 1 : 0;

            await this.SetEffectAsync(
                RULE,
                () => effectConfig.Amount,
                x => effectConfig.Amount = x,
                endValue,
                fadeSeconds,
                completesPrevious);
        }

        async UniTask SetEffectAsync(
            string key,
            DOGetter<float> getter,
            DOSetter<float> setter,
            float endValue,
            float fadeSeconds,
            bool completesPrevious)
        {
            Tweener? tweenerField = this.GetTweener(key);
            if (tweenerField != null)
            {
                var prev = tweenerField;
                this.SetTweener(key, null);
                if (completesPrevious)
                {
                    prev.Complete();
                }
                else
                {
                    prev.Kill();
                }
            }
            tweenerField = DOTween.To(
                getter,
                setter,
                Mathf.Clamp01(endValue),
                Mathf.Clamp(fadeSeconds, 0, 60));
            this.SetTweener(key, tweenerField);
            await tweenerField;
        }

        Tweener? GetTweener(string key)
        {
            return
                key == DIRECTIONAL_GHOST ? this._directionalGhostTweener :
                key == GRAY_SCALE ? this._grayScaleTweener :
                key == SEPIA ? this._sepiaTweener :
                key == RULE ? this._ruleFadeTweener :
                null;
        }
        void SetTweener(string key, Tweener? tweener)
        {
            if (key == DIRECTIONAL_GHOST)
            {
                this._directionalGhostTweener = tweener;
            }
            else if (key == GRAY_SCALE)
            {
                this._grayScaleTweener = tweener;
            }
            else if (key == SEPIA)
            {
                this._sepiaTweener = tweener;
            }
            else if (key == RULE)
            {
                this._ruleFadeTweener = tweener;
            }
        }
    }
}
