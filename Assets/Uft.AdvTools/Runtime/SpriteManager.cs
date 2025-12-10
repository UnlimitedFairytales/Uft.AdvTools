#nullable enable

using DG.Tweening;
using Uft.AdvTools.Entities;
using Uft.AdvTools.View;
using Uft.UnityUtils;
using Uft.UnityUtils.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Uft.AdvTools
{
    public class SpriteManager : MonoBehaviour
    {
        public const int IMG_COUNT = 8;

        public static readonly Color TRANSPARENT = new(1, 1, 1, 0);

        // Parameters

        [SerializeField] protected Image[] _imgSpriteList;
        [SerializeField] protected CharacterView[] _characterViewList;
        [SerializeField] protected bool _controlsCharacterGrayout = true; public bool ControlsCharacterGrayout => this._controlsCharacterGrayout;
        [SerializeField] protected Color _grayoutColor = Color.gray;

        // Status

        /// <summary>誰も表示しなくなった時、nullになる</summary>
        protected Character? _lastSpeaker = null;

        // Methods

        protected bool IsAnyCharacterDisplayed()
        {
            var list = this._characterViewList;
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].IsDisplayed) return true;
            }
            return false;
        }

        protected int GetCharacterViewIndex(Character character)
        {
            var list = this._characterViewList;
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].Character == character)
                {
                    return i;
                }
            }
            return -1;
        }

        public int GetSpriteIndex(Sprite sprite)
        {
            for (int i = 0; i < this._imgSpriteList.Length; i++)
            {
                if (this._imgSpriteList[i].sprite == sprite && 0 < this._imgSpriteList[i].color.a)
                {
                    return i;
                }
            }
            return -1;
        }

        public void SetCharacter(Character character, Sprite? sprite, int index, float offsetX, float offsetY, AnchorPreset pivot, float scale, float fadeTime_sec)
        {
            var list = this._characterViewList;
            var i = this.GetCharacterViewIndex(character);
            if (0 <= i)
            {
                list[i].SetCharacterOff(0);
            }
            list[index].SetCharacter(0 <= i, character, sprite, new Vector2(offsetX, offsetY), pivot, scale, fadeTime_sec);
        }

        public void SetCharacterOff(Character character, float fadeTime_sec)
        {
            var list = this._characterViewList;
            var i = this.GetCharacterViewIndex(character);
            if (0 <= i)
            {
                list[i].SetCharacterOff(fadeTime_sec);
                if (!this.IsAnyCharacterDisplayed())
                {
                    this._lastSpeaker = null;
                }
                return;
            }
            DevLog.LogWarning($"[{nameof(SpriteManager)}.{nameof(SetCharacterOff)}] Displayed character is not found : character.CharacterName={character.CharacterName}");
        }

        public void SetSprite(Sprite sprite, int index, float offsetX, float offsetY, AnchorPreset pivot, float scale, float fadeTime_sec)
        {
            var list = this._imgSpriteList;
            var i = this.GetSpriteIndex(sprite);
            if (0 <= i)
            {
                this.SetSpriteOff(sprite, 0);
            }
            var img = list[index];
            var toPos = new Vector2(offsetX, offsetY);
            var isAlreadyDisplayed = 0 <= i;
            img.sprite = sprite;
            img.DOComplete();
            if (isAlreadyDisplayed)
            {
                img.color = Color.white;
            }
            else
            {
                img.color = TRANSPARENT;
                img.DOColor(Color.white, fadeTime_sec);
            }
            img.SetNativeSize();
            img.rectTransform.pivot = pivot.GetPivot();
            img.rectTransform.anchoredPosition = toPos;
            img.rectTransform.localScale = new Vector3(scale, scale, scale);
        }

        public void SetSpriteOff(Sprite sprite, float fadeTime_sec)
        {
            var list = this._imgSpriteList;
            var i = this.GetSpriteIndex(sprite);
            if (0 <= i)
            {
                var prevImg = list[i];
                prevImg.DOComplete();
                if (0 < fadeTime_sec)
                {
                    prevImg.DOColor(TRANSPARENT, fadeTime_sec);
                }
                else
                {
                    prevImg.color = TRANSPARENT;
                }
                return;
            }
            DevLog.LogWarning($"[{nameof(SpriteManager)}] sprite is not found : sprite.name={sprite.name}");
        }

        public Image? GetSpriteImage(Sprite sprite)
        {
            var i = this.GetSpriteIndex(sprite);
            if (0 <= i) return this._imgSpriteList[i];
            return null;
        }

        public CharacterView? GetCharacterView(Character character)
        {
            var i = this.GetCharacterViewIndex(character);
            return 0 <= i ? this._characterViewList[i] : null;
        }

        /// <summary>
        /// 制御する場合、以下の通り<br/>
        /// 1. 指定キャラクターを通常カラーにする(発言の有無は問わない)<br/>
        /// 2. 「手前に発言者がいた」かつ「手前の発言者と異なる」かつ「名前と発言がある」場合、指定キャラクター以外をグレーアウトする。
        /// </summary>
        public void ControlCharacterGrayout(Character currentCharacter, bool hasNameAndText)
        {
            if (!this.ControlsCharacterGrayout) return;

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
                    if (list[i].IsDisplayed && list[i].Character != currentCharacter)
                    {
                        list[i].ToSub(this._grayoutColor);
                    }
                }
            }
            if (hasNameAndText) this._lastSpeaker = currentCharacter;
        }
    }
}
