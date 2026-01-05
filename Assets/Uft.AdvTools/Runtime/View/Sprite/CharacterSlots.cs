#nullable enable

using DG.Tweening;
using System;
using Uft.AdvTools.Entities;
using Uft.UnityUtils;
using Uft.UnityUtils.UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace Uft.AdvTools.View
{
    public class CharacterSlots : MonoBehaviour
    {
        public const string NAME = "[" + nameof(CharacterSlots) + "]";

        // Parameters

        [SerializeField] protected CharacterView[]? _characterViewList;
        [SerializeField] protected bool _controlsCharacterGrayout = true; public bool ControlsCharacterGrayout => this._controlsCharacterGrayout;
        [SerializeField] protected Color _grayoutColor = Color.gray;

        // Status

        /// <summary>誰も表示しなくなった時、nullになる</summary>
        protected Character? _lastSpeaker = null;

        // Methods

        protected bool IsAnyCharacterDisplayed()
        {
            if (this._characterViewList == null) throw new UnassignedReferenceException(nameof(this._characterViewList));

            var list = this._characterViewList;
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].IsOn) return true;
            }
            return false;
        }

        protected int GetOnCharacterViewIndex(Character character)
        {
            if (this._characterViewList == null) throw new UnassignedReferenceException(nameof(this._characterViewList));

            var list = this._characterViewList;
            for (int i = 0; i < list.Length; i++)
            {
                var oi = list[i];
                if (oi.IsOn && Equals(character, oi.KeyObject)) // NOTE: KeyObjectはオーバーライドが機能するようにEquals比較
                {
                    return i;
                }
            }
            return -1;
        }

        public void SetCharacter(Character character, CharacterDetail detail, int i, float? posX, float? posY, float fadeTime_sec)
        {
            if (this._characterViewList == null) throw new UnassignedReferenceException(nameof(this._characterViewList));

            if ((uint)i >= (uint)this._characterViewList.Length) throw new ArgumentOutOfRangeException(nameof(i));

            var sprite = detail.Sprite;
            var instantiated = character.Instantiated;
            Assert.IsTrue(sprite != null || instantiated != null);

            var prevIndex = this.GetOnCharacterViewIndex(character);
            var oi = this._characterViewList[i];
            var isAlreadyDisplayed = 0 <= prevIndex;
            if (isAlreadyDisplayed)
            {
                if (i != prevIndex)
                {
                    var prevOi = this._characterViewList[prevIndex];
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
            var destLayerPrevKey = oi.CastedKey<Character>();
            if (character != destLayerPrevKey && destLayerPrevKey != null)
            {
                destLayerPrevKey.ResetLastStatus();
            }

            var pivot = detail.Pivot;
            var scale = detail.Scale;
            var x = posX != null ? posX.Value : oi.RootRectTransform.anchoredPosition.x;
            var y = posY != null ? posY.Value : oi.RootRectTransform.anchoredPosition.y;
            var fromAlpha = isAlreadyDisplayed ? 1 : 0;
            if (sprite != null)
            {
                oi.Set(
                    true,
                    character,
                    sprite,
                    pivot.GetPivot(),
                    scale,
                    new Vector2(x, y),
                    new Vector2(detail.OffsetX, detail.OffsetY),
                    fromAlpha,
                    1.0f,
                    isAlreadyDisplayed ? 0 : fadeTime_sec);
            }
            else if (instantiated != null)
            {
                oi.Set(
                    true,
                    character,
                    instantiated,
                    pivot.GetPivot(),
                    scale,
                    new Vector2(x, y),
                    new Vector2(detail.OffsetX, detail.OffsetY),
                    fromAlpha,
                    1.0f,
                    isAlreadyDisplayed ? 0 : fadeTime_sec);
            }
        }

        public void SetCharacterOff(Character character, float fadeTime_sec)
        {
            if (this._characterViewList == null) throw new UnassignedReferenceException(nameof(this._characterViewList));

            var i = this.GetOnCharacterViewIndex(character);
            if (0 <= i)
            {
                this._characterViewList[i].Off(fadeTime_sec);
                if (!this.IsAnyCharacterDisplayed())
                {
                    this._lastSpeaker = null;
                }
                return;
            }
            DevLog.LogWarning($"{NAME} Displayed character is not found : character.CharacterName={character.CharacterName}");
        }

        public CharacterView? GetCharacterView(Character character)
        {
            if (this._characterViewList == null) throw new UnassignedReferenceException(nameof(this._characterViewList));

            var i = this.GetOnCharacterViewIndex(character);
            if (0 <= i) return this._characterViewList[i];
            return null;
        }

        /// <summary>
        /// 制御する場合、以下の通り<br/>
        /// 1. 指定キャラクターを通常カラーにする(発言の有無は問わない)<br/>
        /// 2. 「手前に発言者がいた」かつ「手前の発言者と異なる」かつ「名前と発言がある」場合、指定キャラクター以外をグレーアウトする。
        /// </summary>
        public void ControlCharacterGrayout(Character currentCharacter, bool hasNameAndText)
        {
            if (this._characterViewList == null) throw new UnassignedReferenceException(nameof(this._characterViewList));

            if (!this._controlsCharacterGrayout) return;

            var cView = this.GetCharacterView(currentCharacter);
            if (cView != null)
            {
                cView.ToMain();
            }
            if (this._lastSpeaker != null && this._lastSpeaker != currentCharacter && hasNameAndText)
            {
                var list = this._characterViewList;
                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i].IsOn && !Equals(currentCharacter, list[i].KeyObject)) // NOTE: KeyObjectはオーバーライドが機能するようにEquals比較
                    {
                        list[i].ToSub(this._grayoutColor);
                    }
                }
            }
            if (hasNameAndText) this._lastSpeaker = currentCharacter;
        }
    }
}
