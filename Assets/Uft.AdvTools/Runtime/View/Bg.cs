#nullable enable

using DG.Tweening;
using System;
using Uft.UnityUtils.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Uft.AdvTools.View
{
    public class Bg : MonoBehaviour
    {
        static readonly Color transparent = new(1, 1, 1, 0);

        // Parameters

        [SerializeField] protected Image? _imgBg1;
        [SerializeField] protected Image? _imgBg2;

        // Methods

        public void ChangeBg(Sprite? sprite, float offsetX, float offsetY, AnchorPreset pivot, float scale, float fadeTime_sec)
        {
            if (this._imgBg1 == null || this._imgBg2 == null) throw new InvalidOperationException($"{nameof(this._imgBg1)}, {nameof(this._imgBg2)} are required.");

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

        public Image GetBgImg()
        {
            if (this._imgBg1 == null || this._imgBg2 == null) throw new InvalidOperationException($"{nameof(this._imgBg1)}, {nameof(this._imgBg2)} are required.");

            return this._imgBg2;
        }
    }
}
