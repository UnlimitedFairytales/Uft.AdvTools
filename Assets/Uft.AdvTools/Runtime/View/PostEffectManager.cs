#nullable enable

using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using System.Threading;
using Uft.FadeEffects;
using Uft.UnityUtils;
using Uft.UnityUtils.Common;
using UnityEngine;
using UnityEngine.Rendering;

namespace Uft.AdvTools.View
{
    public class PostEffectManager : MonoBehaviour
    {
        const string DIRECTIONAL_GHOST = "DIRECTIONAL_GHOST";
        const string GrayScale = "GrayScale";
        const string Sepia = "Sepia";
        const string RULE = "RULE";

        public const string FADE_HORIZONTAL = "FadeHorizontal";
        public const string FADE_VERTICAL = "FadeVertical";
        public const string MOSES_H = "MosesH";
        public const string MOSES_V = "MosesV";
        public const string CLOUD = "Cloud";

        [SerializeField] protected SimplePostEffectCollection? _wideCameraSimplePostEffectCollection; public SimplePostEffectCollection WideCameraSimplePostEffectCollection => ThrowIf.Unassigned(this._wideCameraSimplePostEffectCollection);
        public SimplePostEffectConfig WideCameraDirectionalGhostPostEffect
        {
            get
            {
                var index =  0;
                if (GraphicsSettings.currentRenderPipeline == null) index += 10;
                return this.WideCameraSimplePostEffectCollection.SimplePostEffects[index];
            }
        }
        public SimplePostEffectConfig WideCameraGrayscalePostEffect
        {
            get
            {
                var index =  1;
                if (GraphicsSettings.currentRenderPipeline == null) index += 10;
                return this.WideCameraSimplePostEffectCollection.SimplePostEffects[index];
            }
        }
        public SimplePostEffectConfig WideCameraSepiaPostEffect
        {
            get
            {
                var index =  2;
                if (GraphicsSettings.currentRenderPipeline == null) index += 10;
                return this.WideCameraSimplePostEffectCollection.SimplePostEffects[index];
            }
        }
        public SimplePostEffectConfig WideCameraRuleFadePostEffect
        {
            get
            {
                var index =  3;
                if (GraphicsSettings.currentRenderPipeline == null) index += 10;
                return this.WideCameraSimplePostEffectCollection.SimplePostEffects[index];
            }
        }

        [SerializeField] Texture? _texFadeHorizontal;
        [SerializeField] Texture? _texFadeVertical;
        [SerializeField] Texture? _texMosesH;
        [SerializeField] Texture? _texMosesV;
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

        public async UniTask SetDirectionalGhostAsync(CancellationToken cancellationToken, float endValue, float fadeSeconds, bool completesPrevious = false)
        {
            if (this._advRootRef == null) return;
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(this.destroyCancellationToken, cancellationToken);

            await this.SetEffectAsync(
                cts.Token,
                DIRECTIONAL_GHOST,
                () => this.WideCameraDirectionalGhostPostEffect.Amount,
                x => this.WideCameraDirectionalGhostPostEffect.Amount = x,
                endValue,
                fadeSeconds,
                completesPrevious);
        }

        public async UniTask SetImageEffectAsync(CancellationToken cancellationToken, string imageEffectName, float endValue, float fadeSeconds, bool completesPrevious = false)
        {
            if (this._advRootRef == null) return;
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(this.destroyCancellationToken, cancellationToken);

            if (imageEffectName == Sepia)
            {
                await this.SetEffectAsync(
                    cts.Token,
                    Sepia,
                    () => this.WideCameraSepiaPostEffect.Amount,
                    x => this.WideCameraSepiaPostEffect.Amount = x,
                    endValue,
                    fadeSeconds,
                    completesPrevious);
            }
            else if (imageEffectName == GrayScale)
            {
                await this.SetEffectAsync(
                    cts.Token,
                    GrayScale,
                    () => this.WideCameraGrayscalePostEffect.Amount,
                    x => this.WideCameraGrayscalePostEffect.Amount = x,
                    endValue,
                    fadeSeconds,
                    completesPrevious);
            }
            else
            {
                DevLog.LogWarning($"[{nameof(PostEffectManager)}] Invalid {nameof(imageEffectName)}. e.g. the effect name is case-sensitive.");
            }
        }

        public bool RuleFadeIsVisible() => 0 < this.WideCameraRuleFadePostEffect.Amount;

        public async UniTask SetRuleFadeAsync(CancellationToken cancellationToken, string? ruleName, Color color, float ruleSoftness, float endValue, bool isInvert, float fadeSeconds, bool completesPrevious = false)
        {
            if (this._advRootRef == null) return;
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(this.destroyCancellationToken, cancellationToken);

            var effectConfig =  this.WideCameraRuleFadePostEffect;
            var defaultRule = this._texFadeHorizontal;
            Texture? rule =
                ruleName == FADE_HORIZONTAL ? this._texFadeHorizontal :
                ruleName == FADE_VERTICAL ? this._texFadeVertical :
                ruleName == MOSES_H ? this._texMosesH :
                ruleName == MOSES_V ? this._texMosesV :
                ruleName == CLOUD ? this._texCloud :
                effectConfig.RuleTex != null ? effectConfig.RuleTex :
                defaultRule;

            effectConfig.RuleTex = rule;
            effectConfig.SubTexColor = color;
            effectConfig.Softness = ruleSoftness;
            effectConfig.Invert = isInvert ? 1 : 0;

            await this.SetEffectAsync(
                cts.Token,
                RULE,
                () => effectConfig.Amount,
                x => effectConfig.Amount = x,
                endValue,
                fadeSeconds,
                completesPrevious);
        }

        async UniTask SetEffectAsync(
            CancellationToken cancellationToken,
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
            await tweenerField.WithCancellation(cancellationToken);
        }

        Tweener? GetTweener(string key)
        {
            return
                key == DIRECTIONAL_GHOST ? this._directionalGhostTweener :
                key == GrayScale ? this._grayScaleTweener :
                key == Sepia ? this._sepiaTweener :
                key == RULE ? this._ruleFadeTweener :
                null;
        }
        void SetTweener(string key, Tweener? tweener)
        {
            if (key == DIRECTIONAL_GHOST)
            {
                this._directionalGhostTweener = tweener;
            }
            else if (key == GrayScale)
            {
                this._grayScaleTweener = tweener;
            }
            else if (key == Sepia)
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
