#nullable enable

using DG.Tweening;
using Uft.AdvTools.Entities;
using Uft.UnityUtils.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Uft.AdvTools.View
{
    public class CharacterView : MonoBehaviour
    {
        [SerializeField] protected CanvasGroup? _canvasGroup;
        [SerializeField] protected Image? _img;

        public Character? Character { get; protected set; }
        public bool IsDisplayed => this.Character != null;
        public Image? Image => this._img!;

        public void SetCharacter(bool isAlreadyDisplayed, Character character, Sprite sprite, Vector2 toPos, AnchorPreset pivot, float scale, float fadeTime_sec)
        {
            var img = this._img; if (img == null) return;
            var cg = this._canvasGroup; if (cg == null) return;

            this.Character = character;
            img.sprite = sprite;

            cg.DOComplete();
            if (isAlreadyDisplayed)
            {
                cg.alpha = 1;
            }
            else
            {
                cg.alpha = 0;
                cg.DOFade(1, fadeTime_sec);
            }
            img.SetNativeSize();
            img.rectTransform.pivot = pivot.GetPivot();
            img.rectTransform.anchoredPosition = toPos;
            img.rectTransform.localScale = new Vector3(scale, scale, scale);
        }

        public void SetCharacterOff(float fadeTime_sec)
        {
            var img = this._img; if (img == null) return;
            var cg = this._canvasGroup; if (cg == null) return;

            this.Character = null;
            if (0 < fadeTime_sec)
            {
                cg.DOComplete();
                cg.DOFade(0, fadeTime_sec);
            }
            else
            {
                cg.alpha = 0;
            }
        }

        public void ToMain()
        {
            var img = this._img; if (img == null) return;

            img.DOColor(Color.white, 0.2f);
        }

        public void ToSub(Color grayoutColor)
        {
            var img = this._img; if (img == null) return;

            img.DOColor(grayoutColor, 0.2f);
        }
    }
}
