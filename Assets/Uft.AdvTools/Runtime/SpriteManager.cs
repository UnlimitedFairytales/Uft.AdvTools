#nullable enable

using Uft.AdvTools.Entities;
using Uft.AdvTools.View;
using UnityEngine;

namespace Uft.AdvTools
{
    public class SpriteManager : MonoBehaviour
    {
        public const int IMG_COUNT = 8;

        // Parameters

        [SerializeField] protected SpriteSlots? _spriteSlots;
        [SerializeField] protected CharacterSlots? _characterSlots;

        // Methods

        public int GetOnSpriteIndex(TextureRow row)
            => this._spriteSlots!.GetOnTextureRowIndex(row);

        public void SetCharacter(Character character, CharacterDetail detail, int i, float? posX, float? posY, float fadeTime_sec)
            => this._characterSlots!.SetCharacter(character, detail, i, posX, posY, fadeTime_sec);

        public void SetCharacterOff(Character character, float fadeTime_sec)
            => this._characterSlots!.SetCharacterOff(character, fadeTime_sec);

        public void SetSprite(TextureRow row, int i, float? posX, float? posY, float fadeTime_sec)
            => this._spriteSlots!.SetSprite(row, i, posX, posY, fadeTime_sec);

        public void SetSpriteOff(TextureRow row, float fadeTime_sec)
            => this._spriteSlots!.SetSpriteOff(row, fadeTime_sec);

        public OffsettableImage? GetTextureRowOi(TextureRow sprite)
            => this._spriteSlots!.GetTextureRowOi(sprite);

        public CharacterView? GetCharacterView(Character character)
            => this._characterSlots!.GetCharacterView(character);

        /// <summary>
        /// 制御する場合、以下の通り<br/>
        /// 1. 指定キャラクターを通常カラーにする(発言の有無は問わない)<br/>
        /// 2. 「手前に発言者がいた」かつ「手前の発言者と異なる」かつ「名前と発言がある」場合、指定キャラクター以外をグレーアウトする。
        /// </summary>
        public void ControlCharacterGrayout(Character currentCharacter, bool hasNameAndText)
            => this._characterSlots!.ControlCharacterGrayout(currentCharacter, hasNameAndText);
    }
}
