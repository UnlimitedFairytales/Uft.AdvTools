#nullable enable

using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

namespace Uft.AdvTools
{
    public class PostEffectManager
    {
        const string DIRECTIONAL_GHOST = "DIRECTIONAL_GHOST";
        const string GRAY_SCALE = "GRAY_SCALE";
        const string SEPIA = "SEPIA";

        readonly AdvRoot _advRootRef;
        Tweener? _directionalGhostTweener;
        Tweener? _grayScaleTweener;
        Tweener? _sepiaTweener;

        public PostEffectManager(AdvRoot advRootRef)
        {
            this._advRootRef = advRootRef;
        }

        public async UniTask SetDirectionalGhostAsync(float endValue, float fadeSeconds, bool completesPrevious = false)
        {
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
            await this.SetEffectAsync(
                SEPIA,
                () => this._advRootRef.WideCameraSepiaPostEffect.Amount,
                x => this._advRootRef.WideCameraSepiaPostEffect.Amount = x,
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
            var ease = Ease.OutQuad;
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
                Mathf.Clamp(fadeSeconds, 0, 60))
                .SetEase(ease);
            this.SetTweener(key, tweenerField);
            await tweenerField;
        }

        Tweener? GetTweener(string key)
        {
            return
                key == DIRECTIONAL_GHOST ? this._directionalGhostTweener :
                key == GRAY_SCALE ? this._grayScaleTweener :
                key == SEPIA ? this._sepiaTweener :
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
        }
    }
}
