#nullable enable

using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
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

        const string FADE_HORIZONTAL = "FadeHorizontal";
        const string FADE_VERTICAL = "FadeVertical";
        const string MOSES_H = "MosesH";
        const string MOSES_V = "MosesV";
        const string CLOUD = "Cloud";

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

        public async UniTask SetDirectionalGhostAsync(float endValue, float fadeSeconds, bool completesPrevious = false)
        {
            if (this._advRootRef == null) return;

            await this.SetEffectAsync(
                DIRECTIONAL_GHOST,
                () => this.WideCameraDirectionalGhostPostEffect.Amount,
                x => this.WideCameraDirectionalGhostPostEffect.Amount = x,
                endValue,
                fadeSeconds,
                completesPrevious);
        }

        public async UniTask SetImageEffectAsync(string imageEffectName, float endValue, float fadeSeconds, bool completesPrevious = false)
        {
            if (this._advRootRef == null) return;

            if (imageEffectName == Sepia)
            {
                await this.SetEffectAsync(
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

        public async UniTask SetRuleFadeAsync(string ruleName, Color color, float ruleSoftness, float endValue, bool isInvert, float fadeSeconds, bool completesPrevious = false)
        {
            if (this._advRootRef == null) return;

            Texture? rule =
                ruleName == FADE_HORIZONTAL ? this._texFadeHorizontal :
                ruleName == FADE_VERTICAL ? this._texFadeVertical :
                ruleName == MOSES_H ? this._texMosesH :
                ruleName == MOSES_V ? this._texMosesV :
                ruleName == FADE_HORIZONTAL ? this._texFadeHorizontal :
                ruleName == CLOUD ? this._texCloud :
                null;
            if (rule == null) return;

            var effectConfig = this.WideCameraRuleFadePostEffect;
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
