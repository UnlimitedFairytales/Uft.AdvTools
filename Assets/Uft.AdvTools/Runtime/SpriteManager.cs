#nullable enable

using Uft.AdvTools.Entities;
using Uft.AdvTools.View;
using Uft.UnityUtils;
using Uft.UnityUtils.UI;
using UnityEngine;

namespace Uft.AdvTools
{
    public class SpriteManager : MonoBehaviour
    {
        public static readonly Color TRANSPARENT = new(1, 1, 1, 0);

        public const int IMG_COUNT = 8;

        // Parameters

        [SerializeField] protected SpriteSlots? _spriteSlots;
        [SerializeField] protected CharacterView[]? _characterViewList;
        [SerializeField] protected bool _controlsCharacterGrayout = true; public bool ControlsCharacterGrayout => this._controlsCharacterGrayout;
        [SerializeField] protected Color _grayoutColor = Color.gray;

        // Status

        /// <summary>誰も表示しなくなった時、nullになる</summary>
        protected Character? _lastSpeaker = null;

        // Methods

        protected bool IsAnyCharacterDisplayed()
        {
            if (this._characterViewList == null) return false;

            var list = this._characterViewList;
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].IsDisplayed) return true;
            }
            return false;
        }

        protected int GetCharacterViewIndex(Character character)
        {
            if (this._characterViewList == null) return -1;

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

        public int GetOnSpriteIndex(Sprite sprite) => this._spriteSlots!.GetOnSpriteIndex(sprite);

        public void SetCharacter(Character character, Sprite? sprite, GameObject? instantiated, int index, float offsetX, float offsetY, AnchorPreset pivot, float scale, float fadeTime_sec)
        {
            if (this._characterViewList == null) return;

            var list = this._characterViewList;
            var i = this.GetCharacterViewIndex(character);
            if (0 <= i)
            {
                list[i].SetCharacterOff(0);
            }
            list[index].SetCharacter(0 <= i, character, sprite, instantiated, new Vector2(offsetX, offsetY), pivot, scale, fadeTime_sec);
        }

        public void SetCharacterOff(Character character, float fadeTime_sec)
        {
            if (this._characterViewList == null) return;

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

        public void SetSprite(TextureRow row, int i, float? posX, float? posY, float fadeTime_sec) => this._spriteSlots!.SetSprite(row, i, posX, posY, fadeTime_sec);

        public void SetSpriteOff(Sprite sprite, float fadeTime_sec) => this._spriteSlots!.SetSpriteOff(sprite, fadeTime_sec);

        public OffsettableImage? GetSpriteOi(Sprite sprite) => this._spriteSlots!.GetSpriteOi(sprite);

        public CharacterView? GetCharacterView(Character character)
        {
            if (this._characterViewList == null) return null;

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
            if (this._characterViewList == null) return;
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
