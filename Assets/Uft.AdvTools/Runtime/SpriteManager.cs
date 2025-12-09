using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Linq;
using Uft.AdvTools.Commands;
using Uft.AdvTools.Entities;
using Uft.UnityUtils;
using Uft.UnityUtils.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Uft.AdvTools
{
    public class SpriteManager : MonoBehaviour
    {
        public const int IMG_COUNT = 8;

        static readonly Color TRANSPARENT = new(1, 1, 1, 0);

        // Parameters

        [SerializeField] protected Image[] _imgSpriteList;
        [SerializeField] protected Image[] _imgCharacterList;

        // Status

        protected Character[] _characterImageIndexList = new Character[IMG_COUNT]; // HACK: 本当は _imgCharacterList と統合するべき

        // Methods

        public bool IsCharacterDisplayed(Character character) => this._characterImageIndexList.Contains(character);

        public int? GetSpriteIndex(Sprite sprite)
        {
            for (int i = 0; i < this._imgSpriteList.Length; i++)
            {
                if (this._imgSpriteList[i].sprite == sprite)
                {
                    return i;
                }
            }
            return null;
        }

        public void SetCharacter(Character character, Sprite sprite, int index, float offsetX, float offsetY, AnchorPreset pivot, float scale, float fadeTime_sec)
        {
            var ease = Ease.OutQuad;
            var list = this._imgCharacterList;
            var fromColor = TRANSPARENT;
            var toColor = Color.white;
            var fromPos = new Vector2(offsetX, offsetY);
            var toPos = new Vector2(offsetX, offsetY);
            if (this._characterImageIndexList.Contains(character))
            {
                int i = Array.IndexOf(this._characterImageIndexList, character);
                {
                    var prevImg = list[i];
                    prevImg.DOComplete();
                    prevImg.rectTransform.DOComplete();
                    fromColor = prevImg.color;
                    fromPos = prevImg.rectTransform.anchoredPosition;
                    if (i == index)
                    {
                        prevImg.sprite = sprite;
                        prevImg.rectTransform.pivot = pivot.GetPivot();
                        prevImg.rectTransform.localScale = new Vector3(scale, scale, scale);
                        prevImg.rectTransform.DOAnchorPos(toPos, fadeTime_sec).SetEase(ease);
                        return;
                    }
                    else
                    {
                        this._characterImageIndexList[i] = null;
                        prevImg.color = TRANSPARENT;
                        prevImg.sprite = null;
                    }
                }
            }
            this._characterImageIndexList[index] = character;

            list[index].sprite = sprite;
            list[index].color = fromColor;
            list[index].rectTransform.pivot = pivot.GetPivot();
            list[index].rectTransform.localScale = new Vector3(scale, scale, scale);
            list[index].rectTransform.anchoredPosition = fromPos;
            list[index].SetNativeSize();
            list[index].DOColor(toColor, fadeTime_sec).SetEase(ease);
            list[index].rectTransform.DOAnchorPos(toPos, fadeTime_sec).SetEase(ease);
        }

        public void SetCharacterOff(Character character, float fadeTime_sec)
        {
            var ease = Ease.OutQuad;
            var list = this._imgCharacterList;
            if (this._characterImageIndexList.Contains(character))
            {
                var i = Array.IndexOf(this._characterImageIndexList, character);
                {
                    var prevImg = list[i];
                    prevImg.DOComplete();
                    prevImg.rectTransform.DOComplete();
                    prevImg.DOColor(TRANSPARENT, fadeTime_sec).SetEase(ease);
                    this._characterImageIndexList[i] = null;
                    return;
                }
            }
            DevLog.LogWarning($"[{nameof(SpriteManager)}.{nameof(SetCharacterOff)}] Displayed character is not found : character.CharacterName={character.CharacterName}");
        }

        public void SetSprite(Sprite sprite, int index, float offsetX, float offsetY, AnchorPreset pivot, float scale, float fadeTime_sec)
        {
            var ease = Ease.OutQuad;
            var list = this._imgSpriteList;
            var fromColor = TRANSPARENT;
            var toColor = Color.white;
            var fromPos = new Vector2(offsetX, offsetY);
            var toPos = new Vector2(offsetX, offsetY);
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].sprite == sprite)
                {
                    var prevImg = list[i];
                    prevImg.DOComplete();
                    prevImg.rectTransform.DOComplete();
                    fromColor = prevImg.color;
                    fromPos = prevImg.rectTransform.anchoredPosition;
                    if (i == index)
                    {
                        prevImg.rectTransform.pivot = pivot.GetPivot();
                        prevImg.rectTransform.localScale = new Vector3(scale, scale, scale);
                        prevImg.rectTransform.DOAnchorPos(toPos, fadeTime_sec).SetEase(ease);
                        return;
                    }
                    else
                    {
                        prevImg.color = TRANSPARENT;
                        prevImg.sprite = null;
                    }
                    break;
                }
            }
            list[index].sprite = sprite;
            list[index].color = fromColor;
            list[index].rectTransform.pivot = pivot.GetPivot();
            list[index].rectTransform.localScale = new Vector3(scale, scale, scale);
            list[index].rectTransform.anchoredPosition = fromPos;
            list[index].SetNativeSize();
            list[index].DOColor(toColor, fadeTime_sec).SetEase(ease);
            list[index].rectTransform.DOAnchorPos(toPos, fadeTime_sec).SetEase(ease);
        }

        public void SetSpriteOff(Sprite sprite, float fadeTime_sec)
        {
            var ease = Ease.OutQuad;
            var list = this._imgSpriteList;
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].sprite == sprite)
                {
                    var prevImg = list[i];
                    prevImg.DOComplete();
                    prevImg.rectTransform.DOComplete();
                    prevImg.DOColor(TRANSPARENT, fadeTime_sec).SetEase(ease);
                    return;
                }
            }
            DevLog.LogWarning($"[{nameof(SpriteManager)}] sprite is not found : sprite.name={sprite.name}");
        }

        public Image GetCharacterImage(Character character)
        {
            if (!this.IsCharacterDisplayed(character)) return null;

            int i = Array.IndexOf(this._characterImageIndexList, character);
            return this._imgCharacterList[i];
        }

        public Image GetSpriteImage(Sprite sprite)
        {
            var i = this.GetSpriteIndex(sprite);
            if (i == null) return null;

            return this._imgSpriteList[i.Value];
        }
    }
}
