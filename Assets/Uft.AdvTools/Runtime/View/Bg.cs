using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using Uft.AdvTools.Commands;
using Uft.UnityUtils.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Uft.AdvTools.View
{
    public class Bg : MonoBehaviour
    {
        static readonly Color transparent = new(1, 1, 1, 0);

        // Parameters

        [SerializeField] protected Image _imgBg1;
        [SerializeField] protected Image _imgBg2;

        // Methods

        public void ChangeBg(Sprite sprite, float offsetX, float offsetY, AnchorPreset pivot, float scale, float fadeTime_sec)
        {
            var ease = Ease.OutQuad;

            this._imgBg1.sprite = this._imgBg2.sprite;
            this._imgBg1.rectTransform.pivot = this._imgBg2.rectTransform.pivot;
            this._imgBg1.rectTransform.localScale = this._imgBg2.rectTransform.localScale;
            this._imgBg1.rectTransform.anchoredPosition = this._imgBg2.rectTransform.anchoredPosition;
            this._imgBg1.SetNativeSize();
            this._imgBg1.color = this._imgBg2.color;

            this._imgBg2.sprite = sprite;
            this._imgBg2.rectTransform.pivot = pivot.GetPivot();
            this._imgBg2.rectTransform.localScale = new Vector3(scale, scale, scale);
            this._imgBg2.rectTransform.anchoredPosition = new Vector2(offsetX, offsetY);
            this._imgBg2.SetNativeSize();
            this._imgBg2.color = transparent;

            var imgBg2Color = sprite == null ? transparent : Color.white;

            this._imgBg1.DOColor(transparent, fadeTime_sec).SetEase(ease);
            this._imgBg2.DOColor(imgBg2Color, fadeTime_sec).SetEase(ease);
        }

        public async UniTask TweenAsync(CmdTween.TweenType tweenType, CmdTween.TweenParameter parameter, Ease? ease)
        {
            ease ??= Ease.OutQuad;
            var rt = this._imgBg2.rectTransform;
            switch (tweenType)
            {
                case CmdTween.TweenType.MoveTo:
                case CmdTween.TweenType.MoveFrom:
                    {
                        var tweener = rt
                            .DOAnchorPos(
                                new Vector2(parameter.x ?? rt.anchoredPosition.x, parameter.y ?? rt.anchoredPosition.y),
                                parameter.IsSpeed ? parameter.speed.Value : parameter.time.Value);
                        if (tweenType == CmdTween.TweenType.MoveFrom)
                        {
                            tweener = tweener.From();
                        }
                        await tweener
                            .SetSpeedBased(parameter.IsSpeed)
                            .SetDelay(parameter.delay)
                            .SetEase(ease.Value);
                    }
                    break;
                case CmdTween.TweenType.MoveBy:
                    {
                        await rt
                            .DOAnchorPos(
                                new Vector2(parameter.x ?? 0, parameter.y ?? 0),
                                parameter.IsSpeed ? parameter.speed.Value : parameter.time.Value)
                            .SetSpeedBased(parameter.IsSpeed)
                            .SetDelay(parameter.delay)
                            .SetEase(ease.Value)
                            .SetRelative();
                    }
                    break;
                case CmdTween.TweenType.PunchPosition:
                    {
                        await rt
                            .DOPunchAnchorPos(
                                new Vector2(parameter.x ?? 0, parameter.y ?? 0),
                                parameter.time.Value)
                            .SetDelay(parameter.delay)
                            .SetEase(ease.Value);
                    }
                    break;
                case CmdTween.TweenType.ShakePosition:
                    {
                        await rt
                            .DOShakeAnchorPos(
                                parameter.time.Value,
                                new Vector3(parameter.x ?? 0, parameter.y ?? 0, parameter.z ?? 0),
                                20, 90, false, false)
                            .SetDelay(parameter.delay)
                            .SetEase(ease.Value);
                    }
                    break;
                case CmdTween.TweenType.PunchScale:
                    await rt
                        .DOPunchScale(
                            new Vector3(parameter.x ?? 0, parameter.y ?? 0, 0),
                            parameter.time.Value)
                        .SetDelay(parameter.delay)
                        .SetEase(ease.Value);
                    break;
                case CmdTween.TweenType.ShakeScale:
                    await rt
                        .DOShakeScale(parameter.time.Value,
                             new Vector3(parameter.x ?? 0, parameter.y ?? 0, 0),
                             20, 90, false)
                        .SetDelay(parameter.delay)
                        .SetEase(ease.Value);
                    break;
                default:
                    break;
            }
        }
    }
}
