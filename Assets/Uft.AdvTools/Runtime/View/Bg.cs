#nullable enable

using DG.Tweening;
using System;
using Uft.AdvTools.Entities;
using Uft.UnityUtils.UI;
using UnityEngine;

namespace Uft.AdvTools.View
{
    public class Bg : MonoBehaviour
    {
        static readonly Color TRANSPARENT = new(1, 1, 1, 0);

        // Parameters

        [SerializeField] protected OffsettableImage? _oiBg1;
        [SerializeField] protected OffsettableImage? _oiBg2;

        // Methods

        public void ChangeBg(TextureRow? row, float? posX, float? posY, float fadeTime_sec)
        {
            if (this._oiBg1 == null || this._oiBg2 == null) throw new InvalidOperationException($"{nameof(this._oiBg1)}, {nameof(this._oiBg2)} are required.");

            var img1 = this._oiBg1.Image;
            var img2 = this._oiBg2.Image;

            img1.sprite = img2.sprite;
            img1.rectTransform.pivot = img2.rectTransform.pivot;
            img1.rectTransform.localScale = img2.rectTransform.localScale;
            this._oiBg1.RootRectTransform.anchoredPosition = this._oiBg2.RootRectTransform.anchoredPosition;
            img1.rectTransform.anchoredPosition = img2.rectTransform.anchoredPosition;
            img1.SetNativeSize();
            img1.color = img2.color;

            var pivot = row?.Pivot ?? AnchorPreset.MiddleCenter;
            var scale = row?.Scale ?? 1f;
            var x = posX != null ? posX.Value : this._oiBg2.RootRectTransform.anchoredPosition.x;
            var y = posY != null ? posY.Value : this._oiBg2.RootRectTransform.anchoredPosition.y;

            img2.sprite = row?.Sprite;
            img2.rectTransform.pivot = pivot.GetPivot();
            img2.rectTransform.localScale = new Vector3(scale, scale, scale);
            this._oiBg2.RootRectTransform.anchoredPosition = new Vector2(x, y);
            img2.rectTransform.anchoredPosition = new Vector2(row?.OffsetX ?? 0, row?.OffsetY ?? 0);
            img2.SetNativeSize();
            img2.color = TRANSPARENT;
            var imgBg2Color = row == null ? TRANSPARENT : Color.white;
            img1.DOComplete();
            img1.DOColor(TRANSPARENT, fadeTime_sec);
            img2.DOComplete();
            img2.DOColor(imgBg2Color, fadeTime_sec);
        }

        public OffsettableImage GetBgOi()
        {
            if (this._oiBg1 == null || this._oiBg2 == null) throw new InvalidOperationException($"{nameof(this._oiBg1)}, {nameof(this._oiBg2)} are required.");

            return this._oiBg2;
        }
    }
}
