#nullable enable

using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Uft.AdvTools.View
{
    [RequireComponent(typeof(RectTransform))]
    public class OffsettableImage : MonoBehaviour
    {
        // Parameters

        [SerializeField] protected CanvasGroup? _canvasGroup; public CanvasGroup CanvasGroup => this._canvasGroup!; // NOTE: null-forgiving（警告抑制のみ）。他のメンバで必須チェック済み。
        [SerializeField] protected Image? _image; public Image Image => this._image!; // NOTE: null-forgiving（警告抑制のみ）。他のメンバで必須チェック済み。

        public RectTransform RootRectTransform => (RectTransform)this.transform;
        public bool IsOn { get; set; } = false;

        // Unity events

        void Reset()
        {
            this._canvasGroup = this.GetComponentInChildren<CanvasGroup>(true);
            this._image = this.GetComponentInChildren<Image>(true);
        }

        // Methods

        public virtual void Set<T>(bool isOn, T? keyObject, Sprite? sprite, Vector2 pivot, float scale, Vector2 position, Vector2 offset, float fromAlpha, float toAlpha, float fadeTime_sec)
            where T : class
        {
            if (this._canvasGroup == null) throw new InvalidOperationException($"{nameof(this._canvasGroup)} is required.");
            if (this._image == null) throw new InvalidOperationException($"{nameof(this._image)} is required.");

            this.IsOn = isOn;
            var img = this._image;
            var cg = this._canvasGroup;

            cg.DOComplete();

            img.sprite = sprite;
            img.rectTransform.pivot = pivot;
            if (sprite != null) img.SetNativeSize();
            img.rectTransform.localScale = new Vector3(scale, scale, scale);
            this.RootRectTransform.anchoredPosition = position;
            img.rectTransform.anchoredPosition = offset;

            if (0 < fadeTime_sec)
            {
                cg.alpha = fromAlpha;
                cg.DOFade(toAlpha, fadeTime_sec);
            }
            else
            {
                cg.alpha = toAlpha;
            }
        }

        public virtual void Off(float fadeTime_sec)
        {
            if (this._canvasGroup == null) throw new InvalidOperationException($"{nameof(this._canvasGroup)} is required.");
            if (this._image == null) throw new InvalidOperationException($"{nameof(this._image)} is required.");

            this.IsOn = false;
            var img = this._image;
            var cg = this._canvasGroup;
            var sprite = img.sprite;
            cg.DOComplete();
            if (0 < fadeTime_sec)
            {
                cg.DOFade(0, fadeTime_sec).OnComplete(() =>
                {
                    if (img.sprite == sprite)
                    {
                        img.sprite = null;
                    }
                });
            }
            else
            {
                cg.alpha = 0;
                img.sprite = null;
            }
        }
    }
}
