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

        // Parameters

        [SerializeField] protected OffsettableImage[]? _oiSpriteList;

        // Methods

        public int GetOnTextureRowIndex(TextureRow textureRow)
        {
            if (this._oiSpriteList == null) throw new InvalidOperationException($"{nameof(this._oiSpriteList)} is required.");

            var list = this._oiSpriteList;
            for (int i = 0; i < list.Length; i++)
            {
                var oi = list[i];
                if (oi.IsOn && Equals(textureRow, oi.KeyObject))
                {
                    return i;
                }
            }
            return -1;
        }

        public void SetSprite(TextureRow row, int i, float? posX, float? posY, float fadeTime_sec)
        {
            if (this._oiSpriteList == null) throw new InvalidOperationException($"{nameof(this._oiSpriteList)} is required.");
            if ((uint)i >= (uint)this._oiSpriteList.Length) throw new ArgumentOutOfRangeException(nameof(i));

            var sprite = row.Sprite;
            var instantiated = row.Instantiated;
            var prevIndex = this.GetOnTextureRowIndex(row);
            var oi = this._oiSpriteList[i];
            var isAlreadyDisplayed = 0 <= prevIndex;
            if (isAlreadyDisplayed)
            {
                if (i != prevIndex)
                {
                    var prevOi = this._oiSpriteList[prevIndex];
                    oi.RootRectTransform.DOComplete();
                    prevOi.RootRectTransform.DOComplete();
                    oi.RootRectTransform.anchoredPosition = prevOi.RootRectTransform.anchoredPosition;
                    prevOi.Off(0);
                }
            }
            else
            {
                oi.RootRectTransform.anchoredPosition = Vector2.zero;
            }

            // NOTE: 上書き対象の確認とReset処理
            var destLayerPrevKey = oi.CastedKey<TextureRow>();
            if (!Equals(row, destLayerPrevKey) && destLayerPrevKey != null)
            {
                destLayerPrevKey.ResetLastStatus();
            }

            var pivot = row.Pivot;
            var scale = row.Scale;
            var x = posX != null ? posX.Value : oi.RootRectTransform.anchoredPosition.x;
            var y = posY != null ? posY.Value : oi.RootRectTransform.anchoredPosition.y;
            var fromAlpha = isAlreadyDisplayed ? 1 : 0;
            if (sprite != null)
            {
                oi.Set(
                    true,
                    row,
                    sprite,
                    pivot.GetPivot(),
                    scale,
                    new Vector2(x, y),
                    new Vector2(row.OffsetX, row.OffsetY),
                    fromAlpha,
                    1.0f,
                    isAlreadyDisplayed ? 0 : fadeTime_sec);
            }
            else if (instantiated != null)
            {
                oi.Set(
                    true,
                    row,
                    instantiated,
                    pivot.GetPivot(),
                    scale,
                    new Vector2(x, y),
                    new Vector2(row.OffsetX, row.OffsetY),
                    fromAlpha,
                    1.0f,
                    isAlreadyDisplayed ? 0 : fadeTime_sec);
            }
        }

        public void SetSpriteOff(TextureRow row, float fadeTime_sec)
        {
            if (this._oiSpriteList == null) throw new InvalidOperationException($"{nameof(this._oiSpriteList)} is required.");

            var i = this.GetOnTextureRowIndex(row);
            if (0 <= i)
            {
                this._oiSpriteList[i].Off(fadeTime_sec);
                return;
            }
            DevLog.LogWarning($"{NAME} TextureRow is not found : row.Label={row.Label}");
        }

        public OffsettableImage? GetTextureRowOi(TextureRow row)
        {
            if (this._oiSpriteList == null) throw new InvalidOperationException($"{nameof(this._oiSpriteList)} is required.");

            var i = this.GetOnTextureRowIndex(row);
            if (0 <= i) return this._oiSpriteList[i];
            return null;
        }
    }
}
