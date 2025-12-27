#nullable enable

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

            var prevRootRt = this._oiBg2.RootRectTransform;
            var prevImg = this._oiBg2.Image;
            this._oiBg1.Set(
                false,
                prevImg.sprite,
                prevImg.rectTransform.pivot,
                prevImg.rectTransform.localScale.x,
                prevRootRt.anchoredPosition,
                prevImg.rectTransform.anchoredPosition,
                prevImg.color,
                TRANSPARENT,
                fadeTime_sec);

            var pivot = row?.Pivot ?? AnchorPreset.MiddleCenter;
            var scale = row?.Scale ?? 1f;
            var x = posX != null ? posX.Value : this._oiBg2.RootRectTransform.anchoredPosition.x;
            var y = posY != null ? posY.Value : this._oiBg2.RootRectTransform.anchoredPosition.y;
            var toColor = row == null ? TRANSPARENT : Color.white;
            this._oiBg2.Set(
                row?.Sprite != null,
                row?.Sprite,
                pivot.GetPivot(),
                scale,
                new Vector2(x, y),
                new Vector2(row?.OffsetX ?? 0, row?.OffsetY ?? 0),
                TRANSPARENT,
                toColor,
                fadeTime_sec);
        }

        public OffsettableImage GetBgOi()
        {
            if (this._oiBg1 == null || this._oiBg2 == null) throw new InvalidOperationException($"{nameof(this._oiBg1)}, {nameof(this._oiBg2)} are required.");

            return this._oiBg2;
        }
    }
}
