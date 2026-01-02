#nullable enable

using Uft.AdvTools.Entities;
using Uft.UnityUtils.UI;
using UnityEngine;

namespace Uft.AdvTools.View
{
    public class Bg : MonoBehaviour
    {
        // Parameters

        [SerializeField] protected OffsettableImage? _oiBg1;
        [SerializeField] protected OffsettableImage? _oiBg2;

        // Methods

        public void ChangeBg(TextureRow? row, float? posX, float? posY, float fadeTime_sec)
        {
            if (this._oiBg1 == null) throw new UnassignedReferenceException(nameof(this._oiBg1));
            if (this._oiBg2 == null) throw new UnassignedReferenceException(nameof(this._oiBg2));

            var prevRootRt = this._oiBg2.RootRectTransform;
            var prevImg = this._oiBg2.Image;
            this._oiBg1.Set(
                false,
                (object?)null,
                prevImg.sprite,
                prevImg.rectTransform.pivot,
                prevImg.rectTransform.localScale.x,
                prevRootRt.anchoredPosition,
                prevImg.rectTransform.anchoredPosition,
                this._oiBg2.CanvasGroup.alpha,
                0,
                fadeTime_sec);

            var pivot = row?.Pivot ?? AnchorPreset.MiddleCenter;
            var scale = row?.Scale ?? 1f;
            var x = posX != null ? posX.Value : this._oiBg2.RootRectTransform.anchoredPosition.x;
            var y = posY != null ? posY.Value : this._oiBg2.RootRectTransform.anchoredPosition.y;
            var toAlpha = row == null ? 0 : 1;
            this._oiBg2.Set(
                row?.Sprite != null,
                (object?)null,
                row?.Sprite,
                pivot.GetPivot(),
                scale,
                new Vector2(x, y),
                new Vector2(row?.OffsetX ?? 0, row?.OffsetY ?? 0),
                0,
                toAlpha,
                fadeTime_sec);
        }

        public OffsettableImage GetBgOi()
        {
            if (this._oiBg1 == null) throw new UnassignedReferenceException(nameof(this._oiBg1));
            if (this._oiBg2 == null) throw new UnassignedReferenceException(nameof(this._oiBg2));

            return this._oiBg2;
        }
    }
}
