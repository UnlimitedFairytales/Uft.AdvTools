#nullable enable

using DG.Tweening;
using System;
using Uft.AdvTools.Entities;
using Uft.UnityUtils;
using Uft.UnityUtils.UI;
using UnityEngine;

namespace Uft.AdvTools.View
{
    public class SpriteSlots : MonoBehaviour
    {
        public const string NAME = "[" + nameof(SpriteSlots) + "]";

        public static readonly Color TRANSPARENT = new(1, 1, 1, 0);

        // Parameters

        [SerializeField] protected OffsettableImage[]? _oiSpriteList;

        // Methods

        public int GetOnSpriteIndex(Sprite sprite)
        {
            if (this._oiSpriteList == null) throw new InvalidOperationException($"{nameof(this._oiSpriteList)} is required.");

            for (int i = 0; i < this._oiSpriteList.Length; i++)
            {
                var oi = this._oiSpriteList[i];
                if (oi.IsOn && oi.Image.sprite == sprite)
                {
                    return i;
                }
            }
            return -1;
        }

        public void SetSprite(TextureRow row, int i, float? posX, float? posY, float fadeTime_sec)
        {
            if (this._oiSpriteList == null) throw new InvalidOperationException($"{nameof(this._oiSpriteList)} is required.");

            var sprite = row.Sprite;
            var prevIndex = this.GetOnSpriteIndex(sprite);
            var oi = this._oiSpriteList[i];
            var isAlreadyDisplayed = 0 <= prevIndex;
            // NOTE: prevIndex == i の場合もDOCompleteしてOff。問題が出るようなら後で再調整
            if (isAlreadyDisplayed)
            {
                var prevOi = this._oiSpriteList[prevIndex];
                oi.RootRectTransform.DOComplete();
                prevOi.RootRectTransform.DOComplete();
                oi.RootRectTransform.anchoredPosition = prevOi.RootRectTransform.anchoredPosition;
                prevOi.Off(TRANSPARENT, 0);
            }
            else
            {
                oi.RootRectTransform.anchoredPosition = Vector2.zero;
            }

            var pivot = row.Pivot;
            var scale = row.Scale;
            var x = posX != null ? posX.Value : oi.RootRectTransform.anchoredPosition.x;
            var y = posY != null ? posY.Value : oi.RootRectTransform.anchoredPosition.y;
            var fromColor = isAlreadyDisplayed ? Color.white : TRANSPARENT;
            oi.Set(
                true,
                sprite,
                pivot.GetPivot(),
                scale,
                new Vector2(x, y),
                new Vector2(row.OffsetX, row.OffsetY),
                fromColor,
                Color.white,
                isAlreadyDisplayed ? 0 : fadeTime_sec);
        }

        public void SetSpriteOff(Sprite sprite, float fadeTime_sec)
        {
            if (this._oiSpriteList == null) throw new InvalidOperationException($"{nameof(this._oiSpriteList)} is required.");

            var i = this.GetOnSpriteIndex(sprite);
            if (0 <= i)
            {
                this._oiSpriteList[i].Off(TRANSPARENT, fadeTime_sec);
                return;
            }
            DevLog.LogWarning($"{NAME} sprite is not found : sprite.name={sprite.name}");
        }

        public OffsettableImage? GetSpriteOi(Sprite sprite)
        {
            if (this._oiSpriteList == null) throw new InvalidOperationException($"{nameof(this._oiSpriteList)} is required.");

            var i = this.GetOnSpriteIndex(sprite);
            if (0 <= i) return this._oiSpriteList[i];
            return null;
        }
    }
}
